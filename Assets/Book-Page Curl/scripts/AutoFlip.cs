using UnityEngine;
using System.Collections;
[RequireComponent(typeof(Book))]
public class AutoFlip : MonoBehaviour {
    public FlipMode mode;
    public float pageFlipTime = 1;
    public float timeBetweenPages = 1;
    public float delayBeforeStarting;
    public bool autoStartFlip=true;
    public Book controledBook;
    public int animationFramesCount = 40;
    
    private bool _isFlipping;

    private void Start () {
        if (!controledBook)
            controledBook = GetComponent<Book>();
        if (autoStartFlip)
            StartFlipping();
        controledBook.OnFlip.AddListener(PageFlipped);
	}

    private void PageFlipped()
    {
        _isFlipping = false;
    }

    private void StartFlipping()
    {
        StartCoroutine(FlipToEnd());
    }
    public void FlipRightPage()
    {
        if (_isFlipping) 
            return;
        
        if (controledBook.currentPage >= controledBook.TotalPageCount) 
            return;
        
        _isFlipping = true;
        var frameTime = pageFlipTime / animationFramesCount;
        float xc = (controledBook.EndBottomRight.x + controledBook.EndBottomLeft.x) / 2;
        float xl = ((controledBook.EndBottomRight.x - controledBook.EndBottomLeft.x) / 2) * 0.9f;
        //float h =  ControledBook.Height * 0.5f;
        float h = Mathf.Abs(controledBook.EndBottomRight.y) * 0.9f;
        float dx = (xl)*2 / animationFramesCount;
        StartCoroutine(FlipRtl(xc, xl, h, frameTime, dx));
    }
    public void FlipLeftPage()
    {
        if (_isFlipping) return;
        if (controledBook.currentPage <= 0) return;
        _isFlipping = true;
        float frameTime = pageFlipTime / animationFramesCount;
        float xc = (controledBook.EndBottomRight.x + controledBook.EndBottomLeft.x) / 2;
        float xl = ((controledBook.EndBottomRight.x - controledBook.EndBottomLeft.x) / 2) * 0.9f;
        //float h =  ControledBook.Height * 0.5f;
        float h = Mathf.Abs(controledBook.EndBottomRight.y) * 0.9f;
        float dx = (xl) * 2 / animationFramesCount;
        StartCoroutine(FlipLtr(xc, xl, h, frameTime, dx));
    }
    IEnumerator FlipToEnd()
    {
        yield return new WaitForSeconds(delayBeforeStarting);
        var frameTime = pageFlipTime / animationFramesCount;
        var xc = (controledBook.EndBottomRight.x + controledBook.EndBottomLeft.x) / 2;
        var xl = (controledBook.EndBottomRight.x - controledBook.EndBottomLeft.x) / 2f * 0.9f;
        var h = Mathf.Abs(controledBook.EndBottomRight.y)*0.9f;
        //y=-(h/(xl)^2)*(x-xc)^2          
        //               y         
        //               |          
        //               |          
        //               |          
        //_______________|_________________x         
        //              o|o             |
        //           o   |   o          |
        //         o     |     o        | h
        //        o      |      o       |
        //       o------xc-------o      -
        //               |<--xl-->
        //               |
        //               |
        var dx = xl * 2f / animationFramesCount;
        
        switch (mode)
        {
            case FlipMode.RightToLeft:
                while (controledBook.currentPage < controledBook.TotalPageCount)
                {
                    StartCoroutine(FlipRtl(xc, xl, h, frameTime, dx));
                    yield return new WaitForSeconds(timeBetweenPages);
                }
                break;
            case FlipMode.LeftToRight:
                while (controledBook.currentPage > 0)
                {
                    StartCoroutine(FlipLtr(xc, xl, h, frameTime, dx));
                    yield return new WaitForSeconds(timeBetweenPages);
                }
                break;
        }
    }
    private IEnumerator FlipRtl(float xc, float xl, float h, float frameTime, float dx)
    {
        var x = xc + xl;
        var y = (-h / (xl * xl)) * (x - xc) * (x - xc);
        //var y = -h;

        controledBook.BeginDragRightPageToPoint(new Vector3(x, y, 0));
        for (var i = 0; i < animationFramesCount; i++)
        {
            y = (-h / (xl * xl)) * (x - xc) * (x - xc);
            //y = -h;
            controledBook.UpdateBookRtlToPoint(new Vector3(x, y, 0));
            yield return new WaitForSeconds(frameTime);
            x -= dx;
        }
        controledBook.ReleasePage();
    }
    private IEnumerator FlipLtr(float xc, float xl, float h, float frameTime, float dx)
    {
        float x = xc - xl;
        float y = (-h / (xl * xl)) * (x - xc) * (x - xc);
        controledBook.DragLeftPageToPoint(new Vector3(x, y, 0));
        for (int i = 0; i < animationFramesCount; i++)
        {
            y = (-h / (xl * xl)) * (x - xc) * (x - xc);
            controledBook.UpdateBookLtrToPoint(new Vector3(x, y, 0));
            yield return new WaitForSeconds(frameTime);
            x += dx;
        }
        controledBook.ReleasePage();
    }
}
