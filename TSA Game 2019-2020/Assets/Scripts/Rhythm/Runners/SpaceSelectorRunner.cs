﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpaceSelectorRunner : MonoBehaviour
{
    public Vector3 originalPos;

    public KeyCode key;

    public string button;

    public List<GameObject> selectableSpaces = new List<GameObject>();

    public RhythmRunner rhythmRunner;

    public Sprite normalSprite;
    public Sprite pressSprite;

    public List<Sprite> splashImages;

    private void Start()
    {
        originalPos = transform.localPosition;
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.tag == "Space")
            selectableSpaces.Add(collision.gameObject);
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.tag == "Space")
        {
            selectableSpaces.Remove(collision.gameObject);
            collision.GetComponent<SpaceController>().StartDeathFade();
            rhythmRunner.UpdateNotesMissed(1);
        }
    }

    private void Update()
    {
        if (transform.localPosition != originalPos)
            transform.localPosition = originalPos;
        if (Input.GetButtonDown(button))
        {
            GetComponent<Image>().sprite = pressSprite;

            if (selectableSpaces.Count != 0)
            {
                rhythmRunner.UpdateNotesHit(1);
                SpaceHitAccuracy(selectableSpaces[0]);

                //Removes oldest space in the selectable spaces list
                selectableSpaces[0].GetComponent<SpaceController>().Hit();
                selectableSpaces.RemoveAt(0);
            }
            else
                rhythmRunner.UpdateMissclicks(1);
        }

        if(Input.GetButtonUp(button))
            GetComponent<Image>().sprite = normalSprite;
    }

    public void SpaceHitAccuracy(GameObject space)
    {
        if (space != null)
        {
            float hitAccuracy = ((Vector3.Distance(space.transform.position, transform.position) * 100) / transform.GetComponent<RectTransform>().sizeDelta.y) * 1000;
            if (hitAccuracy >= 40) //Hit accuracy = 0-~90
            {
                rhythmRunner.UpdateScore(0.4f); //Bad hit
                rhythmRunner.badHits++;
            }
            else if (hitAccuracy < 40 && hitAccuracy >= 15)
            {
                rhythmRunner.UpdateScore(0.6f); //Okay hit
                rhythmRunner.okayHits++;
            }
            else if (hitAccuracy < 15 && hitAccuracy >= 8)
            {
                rhythmRunner.UpdateScore(0.8f); //Good hit
                //rhythmRunner.SpawnSplashTitle("Good", Color.cyan);
                rhythmRunner.SpawnSplashImage(splashImages[0]);
                rhythmRunner.goodHits++;
            }
            else if (hitAccuracy < 8)
            {
                rhythmRunner.UpdateScore(1); //Perfect hit
                //rhythmRunner.SpawnSplashTitle("Perfect", Color.green);
                rhythmRunner.SpawnSplashImage(splashImages[1]);
                rhythmRunner.perfectHits++;
            }
            rhythmRunner.UpdateAccuracy(100 - hitAccuracy);
        }
    }
}
