using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    
    public float randomRate = 0.3f;
    public float frequency = 2f;

    private float waited = 0;

    private AISupport AiSupport;
    private List<AIBehaviour> AIs = new List<AIBehaviour>();
    [SerializeField]
    private AIBehaviour bestAI;

    [SerializeField]
    private float bestAiValue;

    // Start is called before the first frame update
    void Start()
    {
        AiSupport = GetComponent<AISupport>();

        foreach (AIBehaviour ai in GetComponents<AIBehaviour>())
            AIs.Add(ai);

    }

    // Update is called once per frame
    void Update()
    {
        waited += Time.deltaTime;

        if (waited < frequency)
        {
            return;
        }

        //Debug.Log("AI Controller");

        bestAI = null;
        bestAiValue = 0f;

        AiSupport.Refresh();

        foreach (AIBehaviour ai in AIs)
        {
            ai.TimePassed += waited;

            float aiValue = ai.GetWeight() * ai.WeightMultiplier + Random.Range(0, randomRate);

            if (aiValue > bestAiValue)
            {
                bestAiValue = aiValue;

                //Debug.Log(bestAiValue);

                bestAI = ai;
            }
        }

        //Debug.Log("Best AI is " + bestAI);
        bestAI.Execute();

        waited = 0;
    }


    
}
