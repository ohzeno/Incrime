## 좋은 git 커밋 메시지 규칙
1. 제목과 본문은 한 줄 띄워 분리하기
2. 제목은 영문 기준 50자 이내로
3. 제목 첫글자를 대문자로
4. 제목 끝에 . 금지
5. 제목은 명령조로
6. Github - 제목(이나 본문)에 이슈 번호 붙이기
7. 본문은 영문 기준 72자마다 줄 바꾸기
8. 본문은 어떻게보다 무엇을, 왜에 맞춰 작성하기

## 프로젝트 생성
`initial commit`

## ADD - 코드,테스트,예제,문서 등 추가
### Add A

A를 추가

```
Add ERR_INSPECTOR_COMMAND error
```

### Add A for B

B를 위해 A를 추가

```
Add documentation for the defaultPort option
Add missing includes for vtune build
Add test for InterpolatorType
Add devDependencies support for templates
```

### Add A to B
B에 A를 추가

```
Add error description to Image onError callback
Add displayName to ActivityIndicator
Add deprecation notice to SwipeableListView
```

## FIX - 버그,에러 수정

### Fix A

A를 수정

```
Fix stat cache
Fix changelog entry
Fix broken jsiexecutor search path
```

### Fix A in B

B의 A를 수정

```
Fix calculation in process.uptime()
Fix build warning in node_report.cc
Fix error condition in Verify::VerifyFinal
Fix typo in callback.cc
```

> Fix typo 는 오타 수정을 의미

## REMOVE - 코드 삭제

### Remove A

A를 삭제

```
Remove fallback cache
Remove unnecessary italics from child_process.md
Remove useless additionnal blur call
Remove unneeded .gitignore entries
Remove unused variable
```

> - Clean, Eliminate로 대체 가능
> - A 앞에 ‘useless’, ‘unneeded’, ‘unused’를 붙히기도 함

### Remove A from B

B에서 A를 삭제

```
Remove absolute path parameter from transformers
```

## REFACTOR - 코드 전면 수정 or 리팩터링

### Refactor A

```
Refactor tick objects prune function
Refactor thread life cycle management
Refactor QueryWrap lifetime management
Refactor argument validation
Refactor thread life cycle management
Refactor MockNativeMethods in Jest
```

## UPDATE - 버전 업데이트 및 코드 수정

에러를 잡는 Fix와 달리 정상적으로 동작하는 코드를 보완하거나 수정할 때 사용

## Update A

```
Update acorn
Update repo docs
Update app icons
Update babelHelpers with Babel 7 support
```

## REVISE - 문서 개정

### Revise A

```
Revise deprecation semverness info in Collaborator Guide
```

## RENAME - 이름 변경

### Rename A to B

A를 B로 이름 변경

```
Rename node-report to report
Rename location to trigger
Rename node-report suite to report
```

[좋은 git 커밋 메시지를 작성하기 위한 7가지 약속](https://meetup.toast.com/posts/106)  
[좋은 git commit 메시지를 위한 영어 사전](https://blog.ull.im/engineering/2019/03/10/logs-on-git.html)  

[출처](https://velog.io/@tjdgus3160/Commit-%EB%A9%94%EC%8B%9C%EC%A7%80-%EA%B7%9C%EC%B9%99)
