﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    #region Singleton

    public static GameManager Instance { get; private set; }

    #endregion

    public Dictionary<GameObject, Health> healthContainer;
    public Dictionary<GameObject, Energy> energyContainer;
    public Dictionary<GameObject, Coin> coinContainer;
    public Dictionary<GameObject, BuffReceiver> buffReceiverContainer;
    public Dictionary<GameObject, Rigidbody2D> rigidbodyContainer;
    public Dictionary<GameObject, ItemComponent> itemContainer;
    public Dictionary<GameObject, InteractableObject> interactableObjectsContainer;
    public Dictionary<GameObject, Checkpoint> checkpointsContainer;
    public Dictionary<GameObject, Actor> actorsContainer;
    public bool isSoundEnabled;
    public bool isDebugMode;
    public ItemDataBase itemDataDataBase;
    public PlayerInventory playerInventory;
    public Player player;
    public Checkpoint currentCheckpoint;
    public GameObject canvas;
    [Header("FLAGS")] 
    [SerializeField] private bool isCheatMode;
    private void Awake()
    {
        Instance = this;
        canvas = GameObject.Find("Canvas");
        if (PlayerPrefs.HasKey("Sound_Enabled"))
            isSoundEnabled = Convert.ToBoolean(PlayerPrefs.GetInt("Sound_Enabled"));
        healthContainer = new Dictionary<GameObject, Health>();
        energyContainer = new Dictionary<GameObject, Energy>();
        coinContainer = new Dictionary<GameObject, Coin>();
        buffReceiverContainer = new Dictionary<GameObject, BuffReceiver>();
        rigidbodyContainer = new Dictionary<GameObject, Rigidbody2D>();
        itemContainer = new Dictionary<GameObject, ItemComponent>();
        interactableObjectsContainer = new Dictionary<GameObject, InteractableObject>();
        checkpointsContainer = new Dictionary<GameObject, Checkpoint>();
        actorsContainer = new Dictionary<GameObject, Actor>();
        canvas.transform.GetComponentInChildren<UIInventoryController>().Init();
    }

    public void Start()
    {
        player.gameObject.SetActive(false);
        StartGame();
    }
    public void StartGame()
    {
        StartCoroutine(StartGameCoroutine());
    }

    private IEnumerator StartGameCoroutine()
    {
        yield return new WaitForSeconds(1f);
        RoomManager.Instance.LoadFirstScene();
    }
    
    public void AddActor(GameObject gameObject, Actor actor)
    {
        actorsContainer.Add(gameObject, actor);
        CheckSceneChangerActive();
    }
    
    public void RemoveActor(GameObject gameObject)
    {
        actorsContainer.Remove(gameObject);
        CheckSceneChangerActive();
    }
    
    /**
    * Выключаем старый чекпоинт и добавляем его в интерактивные объекты
    * С новым наоборот
    */
    public void SetCheckpoint(Checkpoint checkpoint)
    {
        if (currentCheckpoint != null)
        {
            interactableObjectsContainer.Add(currentCheckpoint.gameObject,
                currentCheckpoint.GetComponent<InteractableObject>());
            currentCheckpoint.DisableCheckpoint();
        }

        currentCheckpoint = checkpoint;
        currentCheckpoint.EnableCheckpoint();
        interactableObjectsContainer.Remove(currentCheckpoint.gameObject);
        player.ResetInteract();
    }
    
    

    public void StartDeathScreen()
    {
        StartCoroutine(DeathScreen());
    }
    
    private IEnumerator DeathScreen()
    {
        yield return new WaitForSeconds(1f);
        canvas.GetComponent<FadeScreen>().FadeIn();
        StartCoroutine(EndDeathScreen());
    }

    private IEnumerator EndDeathScreen()
    {
        yield return new WaitForSeconds(3f);
        canvas.GetComponent<FadeScreen>().FadeOut();
    }


    
    public void CheckSceneChangerActive()
    {
        if (actorsContainer.Count(g => g.Key.CompareTag("Enemy")) > 0)
        {
            RoomManager.Instance.sceneChanger.SetActive(false);
        }
        else
        {
            RoomManager.Instance.sceneChanger.SetActive(true);
        }
    }
}