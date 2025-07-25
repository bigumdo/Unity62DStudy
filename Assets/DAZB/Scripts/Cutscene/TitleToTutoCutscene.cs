using System.Collections;
using UnityEngine;
using YUI.Cores;

namespace YUI.Cutscenes
{
    public class TitleToTutoCutscene : MonoBehaviour
    {
        [SerializeField] private GameObject player;

        [Space, Space]

        [SerializeField] private GameObject bed;
        [SerializeField] private Transform bedFront;

        private Animator playerAnimator;

        private void Awake() {
            playerAnimator = player.GetComponentInChildren<Animator>();
        }

        private void Start()
        {
            StartCoroutine(Routine());
        }

        private IEnumerator Routine()
        {
            playerAnimator.SetBool("IS_BATTLE", false);

            
            CameraManager.Instance.FadeOut(0);
            yield return new WaitForSeconds(1f);
            CameraManager.Instance.FadeIn(1.5f);

            float elapsedTime = 0;
            float time = 2.5f;

            playerAnimator.SetFloat("MOVEMENT_Y", 1);

            Vector3 playerStartPos = player.transform.position;

            while (elapsedTime < time)
            {
                player.transform.position = Vector3.Lerp(playerStartPos, bedFront.transform.position, elapsedTime / time);

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            player.transform.position = bedFront.transform.position;
            playerAnimator.SetFloat("MOVEMENT_Y", 0);
            yield return new WaitForSeconds(1f);

            elapsedTime = 0;
            time = 1f;

            float playerStartAngle = player.transform.eulerAngles.z;
            playerStartPos = player.transform.position;

            while (elapsedTime < time)
            {
                player.transform.position = Vector3.Lerp(playerStartPos, bed.transform.position, elapsedTime / time);
                player.transform.eulerAngles = new Vector3(0, 0, Mathf.LerpAngle(playerStartAngle, 90, elapsedTime / time));

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            yield return new WaitForSeconds(2f);

            CameraManager.Instance.FadeOut(2);
            yield return new WaitForSeconds(2f);

            yield return new WaitForSeconds(2f);
            GameManager.Instance.LoadScene("Tutorial 1", false);

            yield return null;
        }
    }
}
