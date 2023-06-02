using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class PersonInteractableActivated : UnityEvent<int, string>{}

[System.Serializable]
public class SoundProjectileExpired : UnityEvent{}

[System.Serializable]
public class ChangedProgress : UnityEvent<float>{}

[System.Serializable]
public class RequestPickupSpawn : UnityEvent<string>{}

public class Player : MonoBehaviour
{
    public static Vector3 personScale;

    public static PersonInteractableActivated m_PersonInteractableActivated;
    public static SoundProjectileExpired m_SoundProjectileExpired;
    public static ChangedProgress m_ChangedProgress;
    public static RequestPickupSpawn m_RequestPickupSpawn;

    [Header("Controls")]
    public float rotateSpeed;

    [Header("People")]
    public bool test;
    public float circleRadius;
    public int currentPeople;
    public int maxPeoplePerCircle = 10;
    public float distanceBetweenPeople = 1.25f;
    public GameObject personPrefab;

    private Transform peopleHolder;

    [Header("Projectile")]
    public GameObject soundProjectile;
    public Transform soundSpawner;

    [Header("Pickups")]
    public GameObject personPickupPrefab;
    public float spawnRadiusPercentage;

    void Start() {
        if (m_PersonInteractableActivated == null)
            m_PersonInteractableActivated = new PersonInteractableActivated();

        if (m_SoundProjectileExpired == null)
            m_SoundProjectileExpired = new SoundProjectileExpired();

        if (m_ChangedProgress == null)
            m_ChangedProgress = new ChangedProgress();

        if (m_RequestPickupSpawn == null)
            m_RequestPickupSpawn = new RequestPickupSpawn();

        m_PersonInteractableActivated.AddListener(AddPerson);

        m_SoundProjectileExpired.AddListener(WhenSoundProjectileExpired);

        m_RequestPickupSpawn.AddListener(RequestPickupSpawn);



        // Person scale is 1 when there are 20 people and radius 4
        personScale = new Vector3(20f/maxPeoplePerCircle*circleRadius/4f, 20f/maxPeoplePerCircle*circleRadius/4f, 1f);

        peopleHolder = transform.Find("PeopleHolder");
        
        AddPerson();
        SpawnSoundProjectile();
    }

    void Update() {
        transform.Rotate(new Vector3(0f, 0f, Input.GetAxis("Horizontal") * rotateSpeed * Time.deltaTime));
        if(test) {
            test = false;
            SpawnPersonPickup();
        }
    }

    void AddPerson(int numberOfPeople, string typeOfPerson) {
        for (int i = 0; i < numberOfPeople; i++)
            AddPerson();
    }

    public void AddPerson() {
        if(currentPeople == maxPeoplePerCircle) return;

        GameObject newPerson = Instantiate(personPrefab, new Vector3(0f, -circleRadius, 0f), Quaternion.identity, peopleHolder);
        newPerson.transform.localScale = personScale;
        newPerson.transform.RotateAround(transform.position, new Vector3(0f, 0f, 1f), currentPeople*(360/maxPeoplePerCircle) + transform.eulerAngles.z);
        currentPeople++;

        m_ChangedProgress.Invoke((float)currentPeople/maxPeoplePerCircle);

        if(currentPeople == maxPeoplePerCircle) {
            // Round over
            Debug.Log("Round over");
        }
        
    }

    void RemovePerson() {
        if(currentPeople == 1) return;

        currentPeople--;
        Destroy(peopleHolder.GetChild(peopleHolder.childCount-1).gameObject);

        m_ChangedProgress.Invoke((float)currentPeople/maxPeoplePerCircle);
    }

    public void SpawnSoundProjectile() {
        soundSpawner.eulerAngles = new Vector3(0, 0, currentPeople/2*(360/maxPeoplePerCircle));
        GameObject projectile = Instantiate(soundProjectile, soundSpawner.position, soundSpawner.rotation);
    }

    void WhenSoundProjectileExpired() {
        SpawnSoundProjectile();
        RemovePerson();
    }

    void RequestPickupSpawn(string desc) {
        if(desc == "person") {
            SpawnPersonPickup();
        }
    }

    void SpawnPersonPickup() {
        float randomAngle = Random.Range(0f, 360f);
        float randomDistance = Mathf.Lerp(0, circleRadius*spawnRadiusPercentage, Random.Range(0f, 1f));

        GameObject newPickup = Instantiate(personPickupPrefab, new Vector2(Mathf.Cos(randomAngle)*randomDistance, Mathf.Sin(randomAngle)*randomDistance), Quaternion.identity);
        
    }
    
}
