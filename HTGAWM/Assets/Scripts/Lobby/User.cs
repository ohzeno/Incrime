using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace User
{
    //User 객체를 담아서 JsonSerialize하기 위한 클래스
    [Serializable]
    public class UsersWrapper
    {
        public User[] users;
    }

    //public 이므로 user_pw와 같은 것을 쓸 때는 일시적으로 사용하고 파기할 것
    [Serializable]
    public class User
    {
        public string user_id;
        public int enter_no;
        public string user_name;
        public string user_pw;
        public string user_email;
    }
}
