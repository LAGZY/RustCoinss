using System;
using Facepunch;
using UnityEngine;

namespace Oxide.Plugins
{
    [Info("MonumentsMarkers", "LAGZYA", "1.0.0")]
    public class MonumentsMarkers : RustPlugin
    {
        private static MonumentsMarkers ins;
        private GameObject objecGameObject = new GameObject();

        void OnServerInitialized()
        {
            ins = this;
            objecGameObject = new GameObject();
        }

        [ChatCommand("testmarker")]
        void Marker(BasePlayer player)
        {
            var marker = objecGameObject.AddComponent<MonumentMarker>();
            var land = new LandmarkInfo();
            land.transform.position = player.transform.position;
            marker.Setup(land);
            marker.text.text = "0";
            marker.gameObject.AddComponent<TestMarker>();
        }

        class TestMarker : MonoBehaviour
        {
            private int text = 1;
            private MonumentMarker t;

            private void Awake()
            {
                t = GetComponent<MonumentMarker>();
            }

            private void FixedUpdate()
            {
                if (!ins.IsLoaded)
                {
                    Destroy(this);
                    t.SetActive(false);
                    return;
                }

                t.text.text = text.ToString();
                text++;
            }
        }
    }
}