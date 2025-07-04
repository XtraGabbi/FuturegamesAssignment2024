using UnityEngine;

namespace Mechadroids {
    public class FinalTrigger : MonoBehaviour {
        private bool hasFinished = false;

        private void OnTriggerEnter(Collider other) {
            if(other.CompareTag("Player") & hasFinished == false) {


                Debug.Log("Reached Goal!!!");

                hasFinished = true;
            }


        }
    }
}
