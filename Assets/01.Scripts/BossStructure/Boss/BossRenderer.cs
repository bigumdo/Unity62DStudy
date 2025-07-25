using System.Collections;
using Unity.AppUI.Core;
using UnityEngine;
using YUI.Cores;
using Color = UnityEngine.Color;

namespace YUI.Agents.Bosses
{
    public enum BossColor
    {
        Base,
        Confusion,
        Cracked
    }

    public class BossRenderer : AgentRenderer
    {
        [SerializeField] private Color _baseColor;
        [SerializeField] private Color _confusionColor;
        [SerializeField] private Color _crackedColor;
        [SerializeField] private ParticleSystem _confusionParticle;
        [SerializeField] private ParticleSystem _counterHitParticle;
        [SerializeField] private ParticleSystem _crackedParticle;

        private readonly string _mainColor = "_MainColor";
        private readonly string _isCounter = "_IsCounter";
        private readonly string _isCracked = "_IsCracked";
        private readonly string _bossIntensity = "_BossIntensity";
        private readonly string _outLineTest = "_OutLineTest";
        private readonly string _blinkValue = "_BlinkValue";

        private Boss _boss;

        public override void Initialize(Agent agent)
        {
            base.Initialize(agent);
            _boss = agent as Boss;
            Renderer.material.SetColor(_mainColor, _baseColor);
            Renderer.material.SetFloat(_isCounter, 0);
            Renderer.material.SetFloat(_bossIntensity, 0);
            Renderer.material.SetFloat(_outLineTest, 0);
        }

        public void CounterHitEffect()
        {
            ParticleSystem particle = Instantiate(_counterHitParticle, _agent.transform.position, Quaternion.identity);
            particle.transform.parent = _agent.transform;
            Vector3 dir = BossManager.Instance.counterDir;
            particle.transform.right = dir;
        }

        public IEnumerator PhaseDissolve(float value, float duration = 0.1f)
        {
            float time = 0;
            SpriteRenderer renderer = _boss.GetCompo<BossRenderer>().Renderer;
            float startValue = renderer.material.GetFloat("_PhaseDissolveValue");
            while (time <= duration)
            {
                time += Time.deltaTime;
                renderer.material.SetFloat("_PhaseDissolveValue"
                    , Mathf.Lerp(startValue, value, time / duration));
                yield return null;
            }
            renderer.material.SetFloat("_PhaseDissolveValue", value);
        }

        public IEnumerator CounterShader(float activeTime)
        {
            float time = 0;
            Renderer.material.SetFloat(_isCounter, 1);
            Renderer.material.SetFloat(_bossIntensity, 0);
            Renderer.material.SetFloat(_outLineTest, 0);
            while (time <= activeTime)
            {
                time += Time.deltaTime;

                Renderer.material.SetFloat(_bossIntensity, Mathf.Lerp(0,5,time / activeTime));
                Renderer.material.SetFloat(_outLineTest, Mathf.Lerp(0, 1, time / activeTime));
                yield return null;
            }
        }

        public IEnumerable SetOutLine(bool onOff, float setTime)
        {
            float time = 0;
            while(time <= setTime)
            {
                time += Time.deltaTime;
                Renderer.material.SetFloat(_outLineTest, Mathf.Lerp(0, 1, time / setTime));
                yield return null;
            }
            Renderer.material.SetFloat(_outLineTest, Mathf.Lerp(0, 1, onOff ? 1 : 0));
        }

        public IEnumerator CounterOff(BossColor setColor, float offTime = 0.1f, float durationTime = 0)
        {
            float checkTime = Time.time;
            float time = 0;
            float animiSpeed = _animator.speed;
            _animator.speed = _animator.speed * 0.5f;
            ParticleSystem particle = Instantiate(_confusionParticle, _agent.transform.position, Quaternion.identity);
            particle.transform.parent = _agent.transform;

            _agent.StartCoroutine(SetColor(setColor, offTime));

            while (time <= offTime)
            {
                time += Time.deltaTime;
                Renderer.material.SetFloat(_bossIntensity, Mathf.Lerp(5, 0, time / offTime));
                Renderer.material.SetFloat(_outLineTest, Mathf.Lerp(1, 0, time / offTime));
                yield return null;
            }
            checkTime = Time.time - checkTime;
            yield return new WaitForSeconds(durationTime - checkTime);
            Destroy(particle);
            _animator.speed = animiSpeed;
            Renderer.material.SetFloat(_isCounter, 0);
        }

        public IEnumerator Cracked()
        {
            yield return PhaseDissolve(0, 0);
            _animator.enabled = false;
            yield return SetColor(BossColor.Cracked, 1);
            Instantiate(_crackedParticle, _agent.transform.position, Quaternion.identity).transform.parent = _agent.transform;
            CameraManager.Instance.ShakeCamera(10, 1, 0.1f);
            Renderer.material.SetFloat(_isCracked, 1);
        }

        public IEnumerator SetColor(BossColor setColor, float changeTime = 0.1f)
        {
            float time = 0;
            Color startColor = Renderer.color;
            Color changeColor = _baseColor;
            switch (setColor)
            {
                case BossColor.Base:
                    changeColor = _baseColor;
                    break;
                case BossColor.Confusion:
                    changeColor = _confusionColor;
                    break;
                case BossColor.Cracked:
                    changeColor = _crackedColor;
                    break;
            }

            while (time < changeTime)
            {
                time += Time.deltaTime;
                Color color = Color.Lerp(startColor, changeColor, time / changeTime);
                Renderer.material.SetColor(_mainColor, color);
                yield return null;
            }
        }

        public IEnumerator BlinkEffect(float duration = 0.1f)
        {
            float time = 0;
            while(time < duration * 0.5f)
            {
                time += Time.deltaTime;
                Renderer.material.SetFloat(_blinkValue, Mathf.Lerp(0,1,time / duration));
                yield return null;
            }
            Renderer.material.SetFloat(_blinkValue, 1);
            time = 0;
            while (time < duration * 0.5f)
            {
                time += Time.deltaTime;
                Renderer.material.SetFloat(_blinkValue, Mathf.Lerp(1, 0, time / duration));
                yield return null;
            }
            Renderer.material.SetFloat(_blinkValue, 0);
        }
    }
}
