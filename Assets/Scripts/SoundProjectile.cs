using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class SoundProjectile : MonoBehaviour
{
    public float startingSpeed = 3;
    public float maxSpeed = 7;
    public float currentSpeed;
    public float maxDistanceFromCenter = 100;

    public int bumpsBetweenPickups = 4;
    public static int bumpsBeforeNextPickup;
    public int maxPickupsActiveAtSameTime = 2;
    public static int currentPickupsActive;

    public int currSprite;
    public List<Sprite> sprites;

    public GameObject vfx;

    private Rigidbody2D rb;

    void Start() {
        Player.m_ChangedProgress.AddListener(ChangedProgress);
        Player.m_PersonInteractableActivated.AddListener(PersonInteractableActivated);
        rb = GetComponent<Rigidbody2D>();

        currentSpeed = startingSpeed;

        rb.velocity = transform.up*startingSpeed;

        bumpsBeforeNextPickup = bumpsBetweenPickups;

        transform.rotation = Quaternion.identity;

        currSprite = Random.Range(0, sprites.Count);
        SetSprite(currSprite);

    }

    void SetSprite(int i) {
        GetComponent<SpriteRenderer>().sprite = sprites[i];
    }

    private void OnCollisionEnter2D(Collision2D other) {
        //if (Vector2.Angle(other.contacts[0].normal, rb.velocity) > 90) return;
        //Debug.Log(Vector2.Angle(other.contacts[0].normal, rb.velocity));
        //Debug.Log(other.contacts[0].normal);
        rb.velocity = other.contacts[0].normal * currentSpeed;
        //Debug.Log(rb.velocity);

        Destroy(Instantiate(vfx, transform.position, Quaternion.identity), 2f);

        if (currentPickupsActive < maxPickupsActiveAtSameTime) {
            currSprite++;
            if(currSprite > sprites.Count-1) currSprite = 0;
            SetSprite(currSprite);
            if(--bumpsBeforeNextPickup == 0) {
                bumpsBeforeNextPickup = bumpsBetweenPickups;
                Player.m_RequestPickupSpawn.Invoke("person");
                currentPickupsActive++;
            }
            Player.m_Bump.Invoke(bumpsBetweenPickups - bumpsBeforeNextPickup);
        }
    }

    IEnumerator HitCooldown() {
        GetComponent<Collider2D>().enabled = false;
        yield return new WaitForSeconds(0.2f);
        GetComponent<Collider2D>().enabled = true;
    }

    void Update() {
        if(Vector3.Distance(Vector3.zero, transform.position) > maxDistanceFromCenter) {
            Player.m_SoundProjectileExpired.Invoke();

            Destroy(gameObject, 5f);
            Destroy(this);
        }
    }

    void ChangedProgress(float progress) {
        ChangeSpeed(Mathf.Lerp(startingSpeed, maxSpeed, progress));
    }

    void ChangeSpeed(float newSpeed) {
        currentSpeed = newSpeed;

        rb.velocity = rb.velocity.normalized*newSpeed;
    }

    void PersonInteractableActivated(int i, int s, Vector2 v) {
        currentPickupsActive--;
    }
}
