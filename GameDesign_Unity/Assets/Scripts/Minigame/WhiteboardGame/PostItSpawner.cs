﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PostItSpawner : MonoBehaviour
{
    [SerializeField]
    public GameObject postItPrefab;
    public PostItTarget[] targets;

    [HideInInspector]
    public float difficulty;

    private PostIt draggedPostIt;
    private Color spawnColor;
    private int targetCount;
    private WhiteboardGame gameController;

    private void Start()
    {
        gameController = FindObjectOfType<WhiteboardGame>();
        targetCount = Mathf.RoundToInt(targets.Length / 2 + (targets.Length / 2 * difficulty)) + 1;

        spawnColor = this.GetComponent<Image>().color;
        
        for(int i = 0; i < targets.Length; i++)
        {
            if(i < targetCount)
            {
                targets[i].spawner = this;
                targets[i].gameObject.SetActive(true);
            }
            else
            {
                targets[i].gameObject.SetActive(false);
            }
        }

    }

    private void Update()
    {
        int correctCount = 0;
        foreach(PostItTarget t in targets)
        {
            if (t.gameObject.activeInHierarchy && t.hasPostIt)
            {
                correctCount++;
                if (correctCount == targetCount) gameController.Answered(this);
            }
        }
    }

    public void updateTimeLimit(float timeLeft, float totalTime)
    {
        if (timeLeft < 0) return;
        foreach(PostItTarget t in targets)
        {
            t.transform.localScale = Vector3.one * timeLeft/totalTime;
        }
    }

    public void OnPointerDown(BaseEventData eventData)
    {
        if (!gameController.enabled || !enabled) return;
        Debug.Log("Spawning PostIt: "+name);
        draggedPostIt = GameObject.Instantiate(postItPrefab, 
                                        Input.mousePosition, 
                                        Quaternion.identity,
                                        this.transform.parent)
                                            .GetComponent<PostIt>();
        draggedPostIt.spawner = this;
        draggedPostIt.GetComponent<Image>().color = spawnColor;
    }

    public void OnPointerUp(BaseEventData eventData)
    {
        if (!gameController.enabled || !enabled) return;
        draggedPostIt.Release();
    }
}
