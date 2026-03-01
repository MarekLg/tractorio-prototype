using Unity.Multiplayer.PlayMode;
using Unity.Netcode;
using UnityEngine;

namespace Testing
{
    [RequireComponent(typeof(NetworkManager))]
    public class MultiplayerTestingInit : MonoBehaviour
    {
        private void Start()
        {
            var networkManager = GetComponent<NetworkManager>();

            if (CurrentPlayer.IsMainEditor)
            {
                networkManager.StartHost();
            }
            else
            {
                networkManager.StartClient();
            }
        }
    }
}