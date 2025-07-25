using System.Collections;
using UnityEngine;
using YUI.Agents.players;
using YUI.Cores;

namespace YUI.Skills {
    [CreateAssetMenu(fileName = "MainSkill", menuName = "Skills/Active/MainSkillNode")]
    public class MainSkill : ActiveSkill {
        [SerializeField] private PlayerShield prefab;
        [SerializeField] private GameObject Effect;

        public override void ExecuteSkill(Player player)
        {
            base.ExecuteSkill(player);

            SoundManager.Instance.PlaySound("SFX_Player_SkillDelete");

            player.StartCoroutine(Routine(player));
        }

        private IEnumerator Routine(Player player)
        {
            float elapsedTime = 0;
            float targetTime = 0.225f;

            player.GetCompo<PlayerSkill>().PassiveSkillExecution(SkillExecutionType.ShieldSkillStart);

            bool isVacuumArea = player.GetCompo<PlayerSkill>().GetVariable("VacuumArea") != null;

            PlayerShield obj = Instantiate(prefab, player.transform.position, Quaternion.identity);
            GameObject effect = Instantiate(Effect, player.transform.position, Quaternion.identity);

            obj.SetPlayer(player);
            
            effect.transform.position = player.transform.position;
            effect.transform.SetParent(player.transform);

            while (elapsedTime < targetTime)
            {
                effect.transform.position = player.transform.position;
                obj.transform.localScale = Vector3.Lerp(Vector3.zero, new Vector3(4, 4), elapsedTime / targetTime);

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            yield return new WaitForSeconds(isVacuumArea ? 5f : 0.1f);

            obj.GetComponent<Collider2D>().enabled = false;

            SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();

            elapsedTime = 0;
            targetTime = 0.1f;

            while (elapsedTime < targetTime)
            {
                obj.transform.localScale = Vector3.Lerp(new Vector3(4, 4), new Vector3(4.5f, 4.5f), elapsedTime / targetTime);
                Color color = sr.color;
                color.a = Mathf.Lerp(1, 0, elapsedTime / targetTime);
                sr.color = color;
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            player.GetCompo<PlayerSkill>().PassiveSkillExecution(SkillExecutionType.ShieldSkillEnd);

            Destroy(obj.gameObject);
            Destroy(effect);
        }
    }
}
