using UnityEngine;
using YUI.Agents;

namespace YUI.Agents.players {
    public class PlayerInteract : MonoBehaviour, IAgentComponent
    {
        [SerializeField] private float radius = 3f;

        private RaycastHit2D[] hits;

        private Player player;
        private IInteractable interactableObject;

        public void Initialize(Agent agent)
        {
            player = agent as Player;

            player.InputReader.InteractEvent += HandleInteractEvent;
        }

        private void Update()
        {
            hits = Physics2D.CircleCastAll(transform.position, radius, Vector2.zero, 0f);

            if (hits.Length == 0)
            {
                interactableObject?.Interactable(false);
                interactableObject = null;
                return;
            }

            IInteractable nearest = null;
            float minDistance = 999;

            for (int i = 0; i < hits.Length; ++i)
            {
                RaycastHit2D hit = hits[i];
                if (hit.transform.TryGetComponent(out IInteractable interactable))
                {
                    float distance = Vector3.Distance(transform.position, hit.transform.position);
                    if (minDistance > distance)
                    {
                        minDistance = distance;
                        nearest = interactable;
                    }
                }
            }

            if (nearest == null || interactableObject != nearest)
            {
                interactableObject?.Interactable(false);
            }


            interactableObject = nearest;
            interactableObject?.Interactable(true);
        }


        private void OnDestroy()
        {
            player.InputReader.InteractEvent -= HandleInteractEvent;
        }

        private void HandleInteractEvent()
        {
            interactableObject?.Interact(player);
        }
    }
}
