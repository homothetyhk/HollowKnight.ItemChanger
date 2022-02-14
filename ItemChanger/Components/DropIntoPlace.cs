using System.Collections;

namespace ItemChanger.Components
{
    /// <summary>
    /// Component which causes a RigidBody to fall vertically and lock position when it lands.
    /// </summary>
    public class DropIntoPlace : MonoBehaviour
    {
        Rigidbody2D rb;
        public event Action OnLand;
        public bool Landed { get; private set; } = false;
        private bool artificialRB = false;

        public void Awake()
        {
            rb = gameObject.GetComponent<Rigidbody2D>();
            if (rb == null)
            {
                rb = gameObject.AddComponent<Rigidbody2D>();
                artificialRB = true;
            }
        }

        public void OnEnable()
        {
            if (Landed) return;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionX;
            AccelerationMonitor am = gameObject.GetComponent<AccelerationMonitor>();
            if (am == null) am = gameObject.AddComponent<AccelerationMonitor>();
            StartCoroutine(DetectLanding(rb, am));
        }

        private IEnumerator DetectLanding(Rigidbody2D rb, AccelerationMonitor am)
        {
            yield return new WaitForSeconds(0.05f); // free fall
            while (am && am.IsFalling())
            {
                yield return null;
            }

            OnLand?.Invoke();
            Landed = true;
            if (artificialRB)
            {
                if (am) Destroy(am);
                if (rb) Destroy(rb);
            }

            rb.constraints = RigidbodyConstraints2D.FreezeAll;
        }
    }
}
