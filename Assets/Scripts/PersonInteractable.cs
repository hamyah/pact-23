using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonInteractable : Interactable
{
    public bool active = false;
    public int type;

    public GameObject vfx;

    private GameObject sprite;

    void Start() {
        GetComponent<AudioSource>().Play();

        sprite = transform.Find("Sprite").gameObject;

        type = Random.Range(1, 5);

        sprite.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Alentejanos/alentejano" + type);


        Vector2 randomPos = new Vector2(Random.Range(15f, 20f), Random.Range(15f, 20f));
        //randomPos.x *= Mathf.Pow(-1, randomPos.x % 2);
        //randomPos.y *= Mathf.Pow(-1, randomPos.y % 2);
        Debug.Log(randomPos.x % 2);
        sprite.transform.position = randomPos;
        LTDescr wobble = LeanTween.rotateLocal(sprite, new Vector3(0, 0, 20f), 0.2f).setRepeat(10).setLoopPingPong();
        LeanTween.move(sprite, transform.position, 2f).setOnComplete(Activate);
    }

    protected override void Execute() {
        if(active) {
            Player.m_PersonInteractableActivated.Invoke(1, type, transform.position);
            Destroy(Instantiate(vfx, transform.position, Quaternion.identity), 2f);
            Destroy(gameObject);
        }
    }

    public void Activate() {
        active = true;
    }
}
