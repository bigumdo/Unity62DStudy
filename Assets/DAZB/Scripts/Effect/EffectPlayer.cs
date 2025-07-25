using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using YUI.ObjPooling;

namespace YUI.Effects {
    public class EffectPlayer : PoolableMono {
        private List<ParticleSystem> particleSys;

        private void Awake() {
            particleSys = GetComponentsInChildren<ParticleSystem>().ToList();
        }

        public override void ResetItem()
        {
            particleSys.ForEach(p => p.Play());

            StartCoroutine(PlayRoutine());
        }

        private IEnumerator PlayRoutine() {
            yield return new WaitForSeconds(particleSys[0].main.duration);
            PoolingManager.Instance.Push(this);
        }
    }
}
