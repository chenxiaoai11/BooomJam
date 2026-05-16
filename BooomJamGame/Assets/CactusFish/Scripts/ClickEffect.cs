using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    Vector3 one;
    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log(this.gameObject.name);
        this.transform.localScale = one * 1.1f;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        this.transform.localScale = one;
    }

    // Start is called before the first frame update
    void Start()
    {
        one = this.transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
