using Server.Models;
using Server.Networking.NetworkEvents;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Server.Networking
{
    /// <summary>
    /// Handles the transfer of all Network events to their
    /// associated processes.
    /// </summary>
    public class NetworkEventHandler : MonoBehaviour
    {
        private NetworkDatagramHandler _datagramHandler;
        private WaitForSeconds _waitForInterval;

        [SerializeField]
        private bool _networkingEnabled = true;

        private const float _playerUpdateIntevalSeconds = 0.1f;

        [SerializeField]
        private PlayerConnections _playerConnections;

        [SerializeField]
        private PlantPlacements _plantPlacements;

        [SerializeField]
        private CritterPlacements _critterPlacements;

        [SerializeField]
        private Transform _playerTransform;

        private int _playerId;
        private string _secret;

        private Queue<DatagramCallback> _callbackQueue;

        private void Awake()
        {
            if(!_networkingEnabled) return;

            _waitForInterval = new WaitForSeconds(_playerUpdateIntevalSeconds);
            _datagramHandler = GetComponent<NetworkDatagramHandler>();

            // Set the datagram handler given, upon recieving a message,
            // to send it to this NetworkEventHandler for parsing
            _datagramHandler.MessageRecieved += (_, callback) =>
                _callbackQueue.Enqueue(callback);

            _callbackQueue = new Queue<DatagramCallback>();
        }

        /// <summary>
        /// Begins the handler, sending the first PlayerJoined message
        /// </summary>
        public void StartHandler()
        {
            if(!_networkingEnabled) return;             
            _datagramHandler.SendDatagram(
                new PlayerJoined
                {
                    Position = new GridPosition
                    {
                        X = PlayerControls.GridPosition.x,
                        Y = PlayerControls.GridPosition.y
                    }
                }.CreateString(), true
            );
        }

        private void Update()
        {
            if(!_networkingEnabled) return;

            // If there is an event in the queue, call it
            while(_callbackQueue.Count > 0)
                TransferEvent(_callbackQueue.Dequeue());
        }

        // Periodically sends ping information to the Server, 
        // with the local Player's GridPosition
        private IEnumerator BeginTransfer()
        {
            while(true)
            {
                yield return _waitForInterval;
                _datagramHandler.SendDatagram(
                    new ClientMovement
                    {
                        CallerInfo = new DataModel<GridPosition>
                        {
                            CallerId = _playerId,
                            Secret = "Secret",
                            Value = new GridPosition
                            {
                                X = PlayerControls.GridPosition.x, 
                                Y = PlayerControls.GridPosition.y
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
        /// <returns>The NetworkEvent, with parsed data</returns>
        private NetworkEvent ParseEvent(string text)
        {
            Debug.Log(text);
            var args = text.Split(new string[] { "::" }, StringSplitOptions.None); 
            
            return args[0] switch
            {
                "ClientMovement" => new ClientMovement(args[1]),
                "PlayerJoined" => new PlayerJoined(args[1]),
                "PlayerLeft" => new PlayerLeft(args[1]),
                "Planted" => new Planted(args[1]),
                "Welcome" => new Welcome(args[1]),
                "CreatedCritter" => new CreatedCritter(args[1]),
                "MovedCritter" => new MovedCritter(args[1]),
                _ => null
            };
        }

        /// <summary>
        /// Convert the parsed Event into an action, and return
        /// a datagram to the Server if applicable
        /// </summary>
        /// <param name="callback">The callback method for returning a datagram to the Server.</param>
        private void TransferEvent(DatagramCallback callback)
        {
            switch(ParseEvent(callback.Data))
            {
                case Welcome welcome:  // On Welcome, get the player Id, update the map with the StateSnapshot
                                       // info and begin the Player position transfer

                    _playerId = welcome.Snapshot.CallerId;
                    _secret = welcome.Snapshot.Secret;
                    _plantPlacements.ImportPlants(welcome.Snapshot.Value.PlantSnapshotData);
                    _critterPlacements.ImportCritters(welcome.Snapshot.Value.CritterSnapshotData);
                    StartCoroutine(BeginTransfer());                            break;

                case ClientMovement movement: // On ClientMovement, update the remote Client's position

                    if(movement.CallerInfo.CallerId == _playerId)               return;
                    _playerConnections.UpdateMarker(movement);                  break;

                case Planted planted: // On Planted, add the new Plant to the PlantPlacements

                    _plantPlacements.Place(planted.Placement.Value);            break;

                case CreatedCritter created: // On CreatedCritter, add the new Critter to the CritterPlacements

                    _critterPlacements.CreateCritter(created.Placement);        break;

                case MovedCritter moved: // On MovedCritter, sync the local Player's CritterPlacements

                    _critterPlacements.MoveCritter(moved.Placement);            break;

                case PlayerLeft left: // On PlayerLeft, remove the Client's marker from the PlayerConnections

                    _playerConnections.RemoveMarker(left.CallerInfo.CallerId);  break;

            }
        }

        /// <summary>
        /// Attempts to place a Plant at the specified position, if there
        /// isn't already one set in the Server.
        /// </summary>
        /// <param name="placement"></param>
        public void TryPlantPlacement(PlantPlacement placement) =>
            _datagramHandler.SendDatagram(
                new Planted
                {
                    Placement = new DataModel<PlantPlacement>
                    {
                        CallerId = _playerId,
                        Secret = _secret,
                        Value = placement
                    }
                }.CreateString(), true
            );
    }
}