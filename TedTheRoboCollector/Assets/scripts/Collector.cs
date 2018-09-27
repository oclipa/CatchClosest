using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// A collecting game object
/// </summary>
public class Collector : MonoBehaviour
{
	#region Fields

    // targeting support
    SortedList<Target> targets = new SortedList<Target>();
    Target targetPickup = null;

    // movement support
	const float BaseImpulseForceMagnitude = 2.0f;
    const float ImpulseForceIncrement = 0.3f;
	
	// saved for efficiency
    Rigidbody2D rb2d;

    private float currentClosestDistance = float.MaxValue;

    // circling behaviour
    private bool circling;
    private float rotateSpeed = 5f;
    private float radius = 0.01f;
    private Vector2 centre;
    private float angle;

    #endregion

    #region Methods

    /// <summary>
    /// Use this for initialization
    /// </summary>
    void Start()
    {
		// center collector in screen
		Vector3 position = transform.position;
		position.x = 0;
		position.y = 0;
		position.z = 0;
		transform.position = position;

		// save reference for efficiency
		rb2d = GetComponent<Rigidbody2D>();

        // add as listener for pickup spawned event
        EventManager.AddListener(AddToList);
	}

    void Update()
    {
        if (circling)
        {
            transform.position = getCirclingPosition();
        }
    }

    private void startCircling()
    {
        centre = transform.position;
        circling = true;
        radius = 0.01f;
    }

    private Vector3 getCirclingPosition()
    {
        angle += rotateSpeed * Time.deltaTime;

        var offset = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle)) * radius;
        return centre + offset;
    }

    void FixedUpdate()
    {
        if (radius < 0.5f)
            radius += 0.001f;
    }

    /// <summary>
    /// Called when another object is within a trigger collider
    /// attached to this object
    /// </summary>
    /// <param name="other"></param>
    void OnTriggerStay2D(Collider2D other)
    {
        if (targets.Count > 0)
        {
            // only respond if the collision is with the target pickup
            if (other.gameObject == targetPickup.GameObject)
            {
                // remove collected pickup from list of targets and game
                int index = targets.IndexOf(targetPickup);
                targets.RemoveAt(index);
                Destroy(targetPickup.GameObject);

                // update distances for all targets
                int count = targets.Count;
                for (int i = 0; i < count; i++)
                {
                    targets[i].UpdateDistance(transform.position);
                }
                targets.Sort();

                // go to next target if there is one
                if (targets.Count > 0)
                {
                    SetTarget(targets[count - 1]);
                    circling = false;
                }
                else
                    startCircling();
            }
        } else {
            SetTarget(null);
        }
    }

    void AddToList(GameObject gameObjectToAdd)
    {
        Target target = new Target(gameObjectToAdd, transform.position);
        targets.Add(target);

        if (currentClosestDistance > target.Distance)
        {
            currentClosestDistance = target.Distance;
            SetTarget(target);
        }
    }

	/// <summary>
	/// Sets the target pickup to the provided pickup
	/// </summary>
	/// <param name="pickup">Pickup.</param>
	void SetTarget(Target pickup)
    {
        targetPickup = pickup;

        if (targetPickup != null)
            GoToTargetPickup();
    }

    /// <summary>
    /// Starts the teddy bear moving toward the target pickup
    /// </summary>
    void GoToTargetPickup()
    {
        if (targetPickup != null)
        {
            int numOfPicks = GameObject.FindGameObjectsWithTag("Pickup").Length;

            // calculate direction to target pickup and start moving toward it
            Vector2 direction = new Vector2(
                targetPickup.GameObject.transform.position.x - transform.position.x,
                targetPickup.GameObject.transform.position.y - transform.position.y);
            direction.Normalize();
            rb2d.velocity = Vector2.zero;
            Vector2 force = direction * BaseImpulseForceMagnitude * numOfPicks;
            rb2d.AddForce(force,
                ForceMode2D.Impulse);
        }
    }

    #endregion
}
