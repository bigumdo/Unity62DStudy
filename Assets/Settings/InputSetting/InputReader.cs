using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using YUI.StatusEffects;

namespace YUI.Agents.players
{

    public enum InputType
    {
        None,
        Dash,
        Attack,
        SlowMode,
        Skill,
        Interect,
    }

    [CreateAssetMenu(fileName = "InputReader", menuName = "SO/Input/Player")]
    public class InputReader : ScriptableObject, Controls.IPlayerActions
    {
        public PlayerMoveDir Movement { get; private set; } = PlayerMoveDir.RIGHT;
        public Vector2 MovementVector { get; private set; } = Vector2.zero;
        public bool SlowMode { get; private set; } = false;

        private Controls controls;

        public PlayerMoveDir PrevMovement { get; private set; }

        public event Action<PlayerMoveDir> MovementEvent;
        public event Action<bool> DashEvent;
        public event Action<bool> AttackEvent;
        public event Action UIAttackEvent;


        public event Action MainSkillEvent;
        public event Action SubSkillEvent;
        public event Action CounterEvent;
        public event Action InteractEvent;


        //Input

        private Dictionary<InputType, bool> _inputs = new();

        private void OnEnable()
        {
            if (controls == null)
            {
                controls = new Controls();
                controls.Player.SetCallbacks(this);
            }
            controls.Player.Enable();

            foreach (InputType input in Enum.GetValues(typeof(InputType)))
            {
                _inputs[input] = true;
            }
        }

        public void Init()
        {
            if (controls == null)
            {
                controls = new Controls();
                controls.Player.SetCallbacks(this);
            }
            controls.Player.Enable();

            foreach (InputType input in Enum.GetValues(typeof(InputType)))
            {
                _inputs[input] = true;
            }
        }

        public void ResetEvents() {
            Movement = PlayerMoveDir.RIGHT;
            SlowMode = false;
            
            MainSkillEvent = null;
            MovementEvent = null;
            InteractEvent = null;
            DashEvent = null;
            AttackEvent = null;
            SubSkillEvent = null;
            CounterEvent = null;
            InteractEvent = null;
        }

        private void OnDisable()
        {
            ResetEvents();
            Enable(false);
        }

        public void Enable(bool enable)
        {
            if (enable) controls.Player.Enable();
            else controls.Player.Disable();
        }

        public void SetMovement(PlayerMoveDir movement)
        {
            Movement = movement;
            PrevMovement = movement;
        }

        public void SetSlowMode(bool slowMode)
        {
            SlowMode = slowMode;

            if (!SlowMode)
            {
                Movement = PrevMovement;
            }
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            //if (!GetInput(InputType.Move))
            //    return;

            Vector2 value = context.ReadValue<Vector2>();

            MovementVector = value;

            PlayerMoveDir? newDir = null;

            if (value == Vector2.zero)
                return;

            if (Mathf.Abs(value.x) > Mathf.Abs(value.y))
                newDir = value.x > 0 ? PlayerMoveDir.RIGHT : PlayerMoveDir.LEFT;
            else
                newDir = value.y > 0 ? PlayerMoveDir.UP : PlayerMoveDir.DOWN;

            if (newDir.HasValue && newDir.Value != Movement)
            {
                Movement = newDir.Value;
                PrevMovement = Movement;
                MovementEvent?.Invoke(Movement);
            }
        }


        public void OnSlowMode(InputAction.CallbackContext context)
        {
            if (!GetInput(InputType.SlowMode)) return;

            if (StatusEffectManager.Instance.CheckStatusEffect(StatusEffectType.ForceSlowMode))
            {
                return;
            }

            if (context.performed)
            {
                SlowMode = true;
            }
            else
            {
                Movement = PrevMovement;

                SlowMode = false;
            }
        }

        public void OnDash(InputAction.CallbackContext context)
        {
            if (!GetInput(InputType.Dash)) return;

            if (context.performed)
            {
                DashEvent?.Invoke(true);
            }
            else
            {
                DashEvent?.Invoke(false);
            }
        }

        public void OnAttack(InputAction.CallbackContext context)
        {
            if (!GetInput(InputType.Attack)) return;

            if (context.performed)
            {
                AttackEvent?.Invoke(true);
            }
            else
            {
                AttackEvent?.Invoke(false);
            }

            if (context.canceled)
            {
                AttackEvent?.Invoke(false);
            }

            if(context.canceled)
            {
                UIAttackEvent?.Invoke();
            }
        }

        public void OnMainSkill(InputAction.CallbackContext context)
        {
            if (!GetInput(InputType.Skill)) return;

            if (context.performed)
            {
                MainSkillEvent?.Invoke();
            }
        }

        public void OnCounter(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                CounterEvent?.Invoke();
            }
        }

        public void OnInteract(InputAction.CallbackContext context)
        {
            if (!GetInput(InputType.Interect)) return;

            if (context.performed)
            {
                InteractEvent?.Invoke();
            }
        }


        // About Input limits

        public void SetInput(InputType input, bool enabled)
        {
            _inputs[input] = enabled;
        }

        // For Using InputType is Active State
        public bool GetInput(InputType input)
        {
            return _inputs.TryGetValue(input, out var value) && value;
        }

        public void SetAllInput(bool value)
        {
            var keys = _inputs.Keys.ToList();
            foreach (var key in keys)
            {
                _inputs[key] = value;
            }
        }
    }
}
