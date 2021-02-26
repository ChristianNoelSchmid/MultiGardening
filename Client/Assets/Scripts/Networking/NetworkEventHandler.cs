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
        private readonly WaitForSeconds _waitForTenthOfSecond = new WaitForSeconds(0.1f);

        [SerializeField]
        private Rigidbody2D _playerRb2d;
        private int _playerId;

        [SerializeField]
        private RemoteMovement _templateRemoteMovement;
        private Dictionary<int, RemoteMovement> _remoteMovements;
        private Queue<DatagramCallback> _callbackQueue;

        private void Awake()
        {
            _datagramHandler = GetComponent<NetworkDatagramHandler>();
            _datagramHandler.MessageRecieved += (_, callback) =>
            {
                _callbackQueue.Enqueue(callback);
            };
            _callbackQueue = new Queue<DatagramCallback>();
            _remoteMovements = new Dictionary<int, RemoteMovement>();
        }

        private void Start()
        {
            _datagramHandler.SendDatagram(
                new PlayerJoined
                {
                    ActorMovement = new ActorMovement
                    {
                        Position = Tuple.Create(0f, 0f),
                        IsFlipped = false
                    }
                }.CreateString(), true
            );
            _templateRemoteMovement.gameObject.SetActive(false);
        }

        private void Update()
        {
            while(_callbackQueue.Count > 0)
                TransferEvent(_callbackQueue.Dequeue());
        }

        private IEnumerator BeginTransfer()
        {
            while(true)
            {
                yield return _waitForTenthOfSecond;
                _datagramHandler.SendDatagram(
                    new Pinged
                    {
                        CallerInfo = new DataModel<ActorMovement>
                        {
                            CallerId = _playerId,
                            Secret = "Secret",
                            Value = new ActorMovement
                            {
                                Position = Tuple.Create(_playerRb2d.position.x, _playerRb2d.position.y),
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
                    StartCoroutine(BeginTransfer());
                    break;
                case Pinged pinged:
                    int id = pinged.CallerInfo.CallerId;
                    if(pinged.CallerInfo.CallerId == _playerId)
                        break;

                    if (!_remoteMovements.ContainsKey(id))
                    {
                        _templateRemoteMovement.gameObject.SetActive(true);
                        var newRemote = Instantiate(_templateRemoteMovement.gameObject, null).GetComponent<RemoteMovement>();
                        _templateRemoteMovement.gameObject.SetActive(false);

                        _remoteMovements.Add(id, newRemote);
                    }

                    _remoteMovements[id].SetActorMovement(pinged.CallerInfo.Value);
                    break;
            }
        }
    }
}