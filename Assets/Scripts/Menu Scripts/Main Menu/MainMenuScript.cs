using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuScript : MonoBehaviour
{

    [SerializeField] private GameObject levelMenu;
    [SerializeField] private float animationLength;
    [SerializeField] private float animationSpeed;
    [SerializeField] private float smoothDampTimeChange;
    private Vector2 offset;
    private Animator animator;
    

    
    private void Start() {

        offset = new Vector2(9, 0);
        animator = GetComponent<Animator>();

        animator.SetTrigger("Start Menu Entry");

        //StartCoroutine(menuMove(mainMenu, true, false));
    }

   
    public void Quit() {
        Debug.Log("Game Quit");
        Application.Quit();
        
    }


    public void changeMenu(int from, int to) {
        //StartCoroutine(menuMove(to > from));
        return;
    }

    // private IEnumerator menuMove(bool forward) {

    //     animator.enabled = false;

    //     Transform transform = levelMenu.GetComponent<RectTransform>();
    //     Button[] buttons = GetComponentsInChildren<Button>();

    //     //sets the location target to be left or right of inital position
    //     Vector2 target = transform.position;
    //     target += forward ? -offset : offset;
    //     Debug.Log(transform.position);
    //     Debug.Log(target);

    //     Vector2 velocity = Vector2.zero;
    //     float currentDuration = 0f;
    //     float animationTime = animationSpeed;

    //     //main movement
    //     while (currentDuration <= animationLength) {
    //         transform.position = Vector2.SmoothDamp(transform.position, target, ref velocity, animationTime);
    //         //speeds up animation time slightly to ensure menu reaches it's destination
    //         animationTime -= smoothDampTimeChange;
    //         currentDuration += Time.deltaTime;
    //         yield return null;
    //     }

    //     transform.position = target;
    //     animator.enabled = true;

        
    // }

    
}
