using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Formula 
{
    public static IEnumerator BlinkSelection(GameObject selectionVisual)
    {
        float extraScale = 0.005f;

        selectionVisual.SetActive(true);

        for (float i = 0f; i <= 1f; i += 0.1f)
        {
            selectionVisual.transform.localScale = new Vector3(
                Mathf.Lerp(selectionVisual.transform.localScale.x, selectionVisual.transform.localScale.x + extraScale, Mathf.SmoothStep(0f, 1f, i)),
                Mathf.Lerp(selectionVisual.transform.localScale.y, selectionVisual.transform.localScale.y + extraScale, Mathf.SmoothStep(0f, 1f, i)),
                Mathf.Lerp(selectionVisual.transform.localScale.z, selectionVisual.transform.localScale.z + extraScale, Mathf.SmoothStep(0f, 1f, i)));

            yield return new WaitForSeconds(0.015f);
        }
        for (float i = 0f; i <= 1f; i += 0.1f)
        {
            selectionVisual.transform.localScale = new Vector3(
                Mathf.Lerp(selectionVisual.transform.localScale.x, selectionVisual.transform.localScale.x - extraScale, Mathf.SmoothStep(0f, 1f, i)),
                Mathf.Lerp(selectionVisual.transform.localScale.y, selectionVisual.transform.localScale.y - extraScale, Mathf.SmoothStep(0f, 1f, i)),
                Mathf.Lerp(selectionVisual.transform.localScale.z, selectionVisual.transform.localScale.z - extraScale, Mathf.SmoothStep(0f, 1f, i)));

            yield return new WaitForSeconds(0.015f);
        }

        selectionVisual.SetActive(false);
    }

}
