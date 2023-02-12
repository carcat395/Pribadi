using System;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Extensions;
using Firebase.Firestore;

public class FirestoreManager : MonoBehaviour
{
    private FirebaseFirestore db;
    private DocumentReference masterDocRef;
    
    private int globalResultCount;
    private double[] averageScores = new double[7];

    [SerializeField] bool isConventionalTest;

    private void Start()
    {
        GameManager gm = GameManager.instance;
        db =  FirebaseFirestore.DefaultInstance;
        masterDocRef = db.Collection("result-pool").Document("master");
        gm.onShowResult += (int[] results, Dictionary<string, int> itemResults) =>
        {
            if(GameManager.instance.gameMode == GameManager.GameMode.defaultGame)
                UploadResult(results, itemResults);
            else
                UploadConventionalTestResult(results, itemResults);
        };
        gm.onFinishStartup += CheckUID;
        gm.onResetData += CheckUID;
        GetFirestoreData();
    }

    private void CheckUID()
    {
        if(string.IsNullOrEmpty(GameManager.instance.userID))
            GameManager.instance.userID = "UID-" + globalResultCount;
        
        Debug.Log(GameManager.instance.userID);
    }

    private void GetFirestoreData()
    {
        masterDocRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            UpdateAverageScores(task.Result.ToDictionary());
            GameManager.instance.onFinishStartupProcess.Invoke();
            SetMasterListener();
        });
    }

    private void SetMasterListener()
    {
        masterDocRef.Listen(snapshot => 
        {
            UpdateAverageScores(snapshot.ToDictionary());
        });
    }

    private void UpdateAverageScores(Dictionary<string, object> master)
    {
        globalResultCount = (int)Convert.ToDouble((master[StringRef.RESULT_COUNT]));
        averageScores[0] = Convert.ToDouble(master[StringRef.EXTRAVERSION_AVERAGE]);
        averageScores[1] = Convert.ToDouble((master[StringRef.FRIENDLINESS_AVERAGE]));
        averageScores[2] = Convert.ToDouble((master[StringRef.GREGARIOUSNESS_AVERAGE]));
        averageScores[3] = Convert.ToDouble((master[StringRef.ASSERTIVENESS_AVERAGE]));
        averageScores[4] = Convert.ToDouble((master[StringRef.ACTIVITY_LEVEL_AVERAGE]));
        averageScores[5] = Convert.ToDouble((master[StringRef.EXCITEMENT_SEEKING_AVERAGE]));
        averageScores[6] = Convert.ToDouble((master[StringRef.CHEERFULNESS_AVERAGE]));

        GameManager.instance.globalExtraversionAverage = (float)averageScores[0];
        GameManager.instance.globalAverageCount = globalResultCount;
    }

    public void UploadResult(int[] scores, Dictionary<string, int> itemResults)
    {
        DocumentReference newResultDocRef = db.Collection(StringRef.COLLECTION_RESULT_POOL).Document(GameManager.instance.userID);
        Dictionary<string, object> newResult = new Dictionary<string, object>
        {
            {StringRef.EXTRAVERSION, scores[0]},
            {StringRef.FRIENDLINESS, scores[1]},
            {StringRef.GREGARIOUSNESS, scores[2]},
            {StringRef.ASSERTIVENESS, scores[3]},
            {StringRef.ACTIVITY_LEVEL, scores[4]},
            {StringRef.EXCITEMENT_SEEKING, scores[5]},
            {StringRef.CHEERFULNESS, scores[6]},
            {StringRef.ITEM_RESULTS, itemResults}
        };
        newResultDocRef.SetAsync(newResult);


        Dictionary<string, object> updateMaster = new Dictionary<string, object>
        {
            {StringRef.RESULT_COUNT, globalResultCount+1},
            {StringRef.EXTRAVERSION_AVERAGE, AddAverage(averageScores[0], scores[0])},
            {StringRef.FRIENDLINESS_AVERAGE, AddAverage(averageScores[1], scores[1])},
            {StringRef.GREGARIOUSNESS_AVERAGE, AddAverage(averageScores[2], scores[2])},
            {StringRef.ASSERTIVENESS_AVERAGE, AddAverage(averageScores[3], scores[3])},
            {StringRef.ACTIVITY_LEVEL_AVERAGE, AddAverage(averageScores[4], scores[4])},
            {StringRef.EXCITEMENT_SEEKING_AVERAGE, AddAverage(averageScores[5], scores[5])},
            {StringRef.CHEERFULNESS_AVERAGE, AddAverage(averageScores[6], scores[6])}
        };
        masterDocRef.SetAsync(updateMaster);

        GameManager.instance.onFinishUpload?.Invoke();
    }

    public void UploadConventionalTestResult(int[] scores, Dictionary<string, int> itemResults)
    {
        DocumentReference newResultDocRef = db.Collection(StringRef.COLLECTION_RESULT_POOL_CONVENTIONAL_TEST).Document(new string(GameManager.instance.userID));
        Dictionary<string, object> newResult = new Dictionary<string, object>
        {
            {StringRef.EXTRAVERSION, scores[0]},
            {StringRef.FRIENDLINESS, scores[1]},
            {StringRef.GREGARIOUSNESS, scores[2]},
            {StringRef.ASSERTIVENESS, scores[3]},
            {StringRef.ACTIVITY_LEVEL, scores[4]},
            {StringRef.EXCITEMENT_SEEKING, scores[5]},
            {StringRef.CHEERFULNESS, scores[6]},
            {StringRef.ITEM_RESULTS, itemResults}
        };

        newResultDocRef.SetAsync(newResult);
        GameManager.instance.onFinishUpload?.Invoke();
    }

    private double AddAverage(double averageValue, int newValue)
    {
        float result = Mathf.Round((float)(((averageValue * globalResultCount) + newValue) / globalResultCount + 1) * 100f) / 100f;
        return (double)result;
    }
}
