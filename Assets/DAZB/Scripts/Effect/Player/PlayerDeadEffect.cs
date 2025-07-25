using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using YUI.ObjPooling;

namespace YUI.Agents.players
{
    public class PlayerDeadEffect : MonoBehaviour
    {
        [SerializeField] private float spreadSpeed = 3f;
        [SerializeField] private float gravity = 9f;
        private List<GameObject> effectObjects;

        private void Awake()
        {
            effectObjects = transform.Cast<Transform>().Select(t => t.gameObject).ToList();
            StartRoutine();
        }

        public void StartRoutine()
        {
            StartCoroutine(Routine());
        }

        private IEnumerator Routine()
        {
            float[] angles = { 60f, 0f, -60f, -90f, -120f, -180f };

            for (int i = 0; i < effectObjects.Count && i < angles.Length; i++)
            {
                var rb = effectObjects[i].GetComponent<Rigidbody>();
                rb.linearVelocity = Vector3.zero;

                float rad = -angles[i] * Mathf.Deg2Rad;
                Vector3 dir = new Vector3(Mathf.Cos(rad), 0, Mathf.Sin(rad));

                Debug.Log($"Effect {i} direction: {dir}");

                rb.AddForce(dir * spreadSpeed, ForceMode.Impulse);
            }

            yield return null;
        }

        private void Update()
        {

            for (int i = 0; i < effectObjects.Count; i++)
            {
                Vector3 pos = effectObjects[i].transform.localPosition;
                pos.z -= gravity * Time.deltaTime;
                effectObjects[i].transform.localPosition = pos;
            }
        }
    }
}
