﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectorRunner : MonoBehaviour
{
    public int laneNumber; //0 = left lane, 1 = mid lane, 2 = right lane; defines it's keycode
    public KeyCode key;

    public List<GameObject> selectableNotes = new List<GameObject>();
    public GameObject selectableSlider;
    public bool selectableSliderBeingHit;
    public float sliderHeightChange; //How much the height (not scale) is increased by each FixedUpdate call on the spawned slider; Set to scroll speed by rhythmRunner

    public RhythmRunner rhythmRunner;

    public ParticleSystem noteHitParticle;

    public Sprite normalSprite;
    public Sprite pressSprite;

    private void Start()
    {
        //KEY HARD CODED IN INSPECTOR ON SELECTOR IN NEW MULTI-LANE SYSTEM
        //key = rhythmRunner.laneKeycodes[laneNumber]; //Gets this selector's keycode from it's lane index & the keycode list in RhythmController.cs
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.tag == "Note")
        {
            selectableNotes.Add(collision.gameObject);
        }
        else if (collision.tag == "Slider" && selectableSlider == null)
        {
            selectableSlider = collision.gameObject;
            collision.GetComponent<SliderController>().StartCanBeHitTimer();
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.tag == "Note" && !collision.GetComponent<NoteController>().hasBeenHit)
        {
            selectableNotes.Remove(collision.gameObject);
            collision.GetComponent<NoteController>().StartDeathFade();
            rhythmRunner.UpdateNotesMissed(1);
        }
        else if (collision.tag == "Slider")
        {
            if (!collision.GetComponent<SliderController>().hasBeenHit)
            {
                collision.GetComponent<SliderController>().StartDeathFade();
                rhythmRunner.UpdateNotesMissed(1);
                selectableSlider = null;
            }
            else if(collision.GetComponent<SliderController>().incompleteHit) //Separate from top if b/c notesMissed is called when the slider stops being hit half way instead of doing it here when it is dissapearing
            {
                collision.GetComponent<SliderController>().StartDeathFade();
                selectableSlider = null;
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(key))
        {
            GetComponent<Image>().sprite = pressSprite;

            bool somethingClicked = false; //If false by the end of this if, this click counts as a misclick
            if (selectableNotes.Count != 0)
            {
                somethingClicked = true;
                rhythmRunner.UpdateNotesHit(1);
                rhythmRunner.UpdateScore(1);

                //Removes oldest note in the selectable notes list
                selectableNotes[0].GetComponent<NoteController>().Hit();
                selectableNotes.RemoveAt(0);

                /*CODE TO KILL MULTIPLE NODES WITH SINGLE CLICK
                List<GameObject> notesToRemove = new List<GameObject>();
                foreach (GameObject note in selectableNotes)
                {
                    note.GetComponent<NoteController>().Hit();
                    notesToRemove.Add(note);
                }
                foreach (GameObject note in notesToRemove)
                    selectableNotes.Remove(note);
                notesToRemove.Clear();*/
            }
            
            if(selectableSlider != null && selectableSlider.GetComponent<SliderController>().canBeHit && !selectableSlider.GetComponent<SliderController>().hasBeenHit) //If there is a selectableSlider that can be hit and hasnt been hit yet
            {
                selectableSlider.GetComponent<SliderController>().hasBeenHit = true;
                somethingClicked = true;
                noteHitParticle.Play();
                selectableSliderBeingHit = true;
            }

            if (!somethingClicked)
                rhythmRunner.UpdateMissclicks(1);
        }

        if (Input.GetKeyUp(key))
        {
            GetComponent<Image>().sprite = normalSprite;

            //If you stop hitting a slider mid way
            if (selectableSlider != null && selectableSlider.GetComponent<SliderController>().hasBeenHit)
            {
                noteHitParticle.Stop();
                selectableSlider.GetComponent<SliderController>().incompleteHit = true;
                selectableSliderBeingHit = false;
                rhythmRunner.UpdateNotesMissed(1);
            }
        }
    }

    private void FixedUpdate()
    {
        if (selectableSliderBeingHit)
        {
            rhythmRunner.UpdateScore(0.1f);

            //Move slider's mask parent up, counteracting scroller
            selectableSlider.transform.parent.localPosition = new Vector3(selectableSlider.transform.parent.localPosition.x, selectableSlider.transform.parent.localPosition.y + rhythmRunner.scrollSpeed, selectableSlider.transform.parent.localPosition.z);
            selectableSlider.transform.localPosition = new Vector3(selectableSlider.transform.localPosition.x, selectableSlider.transform.localPosition.y - rhythmRunner.scrollSpeed, selectableSlider.transform.localPosition.z);

            if (-selectableSlider.transform.localPosition.y > selectableSlider.GetComponent<RectTransform>().sizeDelta.y)
            {
                if(!selectableSlider.GetComponent<SliderController>().incompleteHit) //If the slider was hit in it's entirity, give note score
                {
                    rhythmRunner.UpdateNotesHit(1);
                    rhythmRunner.UpdateScore(1);
                }
                selectableSlider.GetComponent<SliderController>().HitDeath();
                selectableSlider = null;
                selectableSliderBeingHit = false;
                noteHitParticle.Stop();
            }
        }
    }
}
