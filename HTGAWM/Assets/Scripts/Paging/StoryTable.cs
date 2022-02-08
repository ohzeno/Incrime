using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoryTable : MonoBehaviour
{
    public Text StoryTxt;
    
    private string story1;
    private string story2;
    private int pagenum = 1;
    public Text pagestring;
    

    void Start()
    {
        story1 = "  6개월 전부터 피해자의 비서 업무를 맡게 된 김비서. 그런데 새로운 상사인 이팀장은 온갖 잡무를 시키며 비서인 자신을 하대하기 일쑤였다. 자신보다 나이가 어린 이상미가 팀장 자리를 꿰찬 건 분명 능력보다 타고난 배경 덕분이라 생각했고 그런 이팀장에 대한 증오와 시샘은 점점 깊어져 갔는데... 두 달 전 자신이 좋아했던 최과장이 그녀와 사귀고 있었음을 알게 되고, 급기야 연봉 인상 대상에서 그녀가 자신만을 제외시켰다는 사실을 알게 되고 이팀장에게 항의했지만 받아들여지지 않자 굴욕감이 극에 달한 김비서. 결국 이팀장을 죽이기로 결심한다.\n\n  우연히 봤던 영화 속 한 장면과 함께 힌트가 떠올랐고 이팀장 방에 있던 스탠드를 이용해 감전사를 시킨 후 사고사로 위장할 계획을 세운다. 이팀장이 밤늦게 출장을 떠날 예정이었던 사건 당일, 사무실이 비는 저녁 시간을 틈타 계획을 실행에 옮기는데... ";
        story2 = "  이팀장의 책상 위 스탠드의 전선 피복을 벗겨 전류가 노출되도록 만든 것. 잠시 후 팀장실로 돌아온 이팀장의 커피에 미리 준비해둔 수면제를 섞은 뒤에 그녀가 약기운에 취해 엎드려 잠들기만을 기다리는데... 그리고 드디어 때가 왔다. 책상에 엎드린 채 깊은 잠에 빠져버린 이팀장을 발견. 김비서는 계획했던 대로 탕비실로 가 탕비실에 있던 가연성 방향제 스프레이와 마이사의 방에서 훔친 라이터로 스프링클러를 향해 불을 쐈는데...\n\n  그 시각 다른 용의자들은, 최과장은 회의실에, 천보안은 시설보안실에, 마이사와 윤사원은 각각 화장실에, 그리고 장대행은 귀가길에 있었는데... 스프링클러가 터지며 물이 쏟아져 나왔고 피복이 벗겨진 구리선에 물이 닿으며 책상 위로 흐르기 시작한 전류는 젖은 책상과 맞닿아있던 이팀장의 팔을 타고 순식간에 심장을 관통, 결국 그녀는 갑작스런 감전에 의한 심장마비로 사망에 이르게 된다. 그 시각 이 모든 것을 계획한 김비서는 모두가 혼잡한 틈을 이용해 사건 현장을 빠져나갔다.";
        pagestring.text = "1/2";
        StoryTxt.text = story1;
    }

    public void toleft()
    {
        if( pagenum != 1){
            pagenum -= 1;
            StoryTxt.text = story1;
            pagestring.text = "1/2";
        }
    }

    public void toright()
    {
        if (pagenum != 2){
            pagenum += 1;
            StoryTxt.text = story2;
            pagestring.text = "2/2";
        }
    }
}
