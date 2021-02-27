using Server.Models;
using Server.Networking.NetworkEvents;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Server.Networking
{
    public class NetworkEventHandler : MonoBehaviour
    {
        private NetworkDatagramHandler _datagramHandler;
        private WaitForSeconds _waitForInterval;

        [SerializeField]
        private bool _networkingEnabled = true;

        [SerializeField]
        private float _playerUpdateIntevalSeconds = 0.1f;

        [SerializeField]
        private PlayerConnections _playerConnections;

        [SerializeField]
        private Transform _playerTransform;
        private int _playerId;

        private Queue<DatagramCallback> _callbackQueue;

        private void Awake()
        {
            if(!_networkingEnabled) return;

            _waitForInterval = new WaitForSeconds(_playerUpdateIntevalSeconds);
            _datagramHandler = GetComponent<NetworkDatagramHandler>();
            _datagramHandler.MessageRecieved += (_, callback) =>
            {
                _callbackQueue.Enqueue(callback);
            };
            _callbackQueue = new Queue<DatagramCallback>();
        }

        private void Start()
        {
            if(!_networkingEnabled) return;            

            _datagramHandler.SendDatagram(
                new PlayerJoined
                {
                    ActorMovement = new ActorMovement
                    {
                        Position = Tuple.Create(_playerTransform.position.x, _playerTransform.position.y),
                        IsFlipped = false
                    }
                }.CreateString(), true
            );
        }

        private void Update()
        {
            if(!_networkingEnabled) return;

            while(_callbackQueue.Count > 0)
                TransferEvent(_callbackQueue.Dequeue());
        }

        private IEnumerator BeginTransfer()
        {
            while(true)
            {
                yield return _waitForInterval;
                _datagramHandler.SendDatagram(
                    new Pinged
                    {
                        CallerInfo = new DataModel<ActorMovement>
                        {
                            CallerId = _playerId,
                            Secret = "Secret",
                            Value = new ActorMovement
                            {
                                Position = Tuple.Create(_playerTransform.position.x, _playerTransform.position.y),
                                IsFlipped = false
                            }
                        }
                    }.CreateString(), false
                );
            }
        }

        /// <summary>
        /// Converts incoming text into an appropriate
        /// NetworkEvent.
        /// </summary>
        /// <param name="text">The string to parse</param>
        /// <returns></returns>
        private NetworkEvent ParseEvent(string text)
        {
            var args = text.Split(new string[] { "::" }, StringSplitOptions.None); 
            
            return args[0] switch
            {
                "PlayerJoined" => new PlayerJoined(args[1]),
                "PlayerLeft" => new PlayerLeft(args[1]),
                "Tilled" => new Tilled(args[1]),
                "Planted" => new Planted(args[1]),
                "Destroyed" => new Destroyed(args[1]),
                "Pinged" => new Pinged(args[1]),
                "Welcome" => new Welcome(args[1]),
                _ => null
            };
        }
        private void TransferEvent(DatagramCallback callback)
        {
            switch(ParseEvent(callback.Data))
            {
                case Welcome welcome: 

                    _playerId = welcome.DataModel.CallerId;
                    StartCoroutine(BeginTransfer());            break;

                case Pinged pinged:

                    if(pinged.CallerInfo.CallerId == _playerId) return;
                    _playerConnections.UpdateMarker(pinged);    break;

            }
        }
    }
}