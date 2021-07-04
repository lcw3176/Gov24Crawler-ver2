# Gov24Crawler-ver2
## 개요
정부24 토지대장 발급 자동화 ver-2

* **시간, 비용 절감**: 토지대장을 발급하는 방법은 여러 가지가 있다. 아주 단순하게 구별하면 남이 하거나, 내가 하거나. 토지대장 1500개를 발급해야 하는 상황에서 구청에 100만원을 주고 맡길 것인지, 자신이 하나하나 발급할 것인지. 둘 다 엄두가 나지 않는다면 이 프로그램이 도움이 될 수 있다.

* **효율성**: 본 프로그램은 정보 수집 후 엑셀파일로 정리도 하는 동시에 발급받은 토지대장을 캡쳐 후 이미지 파일로 저장한다. 차후 추가적인 정보가 필요하다면 저장된 이미지로 간편하게 확인 가능하다.

## 기능
### 토지대장 발급
* 정부24 사이트에서 진행하는 토지대장 발급 신청 자동화
### 데이터 수집
* 발급받은 토지대장을 분석 후 엑셀 파일화
* 필요한 정보를 원하는 칸에 정리 가능
### 토지대장 캡쳐
* 발급받은 토지대장 캡쳐 후 사용자 지정 폴더에 저장
* 파일명은 해당 토지대장의 번지수로 저장됨

## 실행 전 준비요소
### 필수
* AnySign for PC 설치
* 정부24 아이디 (비밀번호 변경 권유 메세지가 뜬다면 변경후 진행할 것)
* Chrome 브라우저 (최신 버전, [업데이트 방법](https://support.google.com/chrome/answer/95414?co=GENIE.Platform%3DDesktop&hl=ko) 참고)
### 옵션
* Visual Studio 2019 (.NET 데스크톱 개발)
* Git 

## 실행법
### 프로젝트로 실행
1. 프로젝트 다운로드 (2가지 방법 중 택 1)
    * git clone https://github.com/lcw3176/Gov24Crawler-ver2.git 명령어 실행
    * [Download Zip](https://github.com/lcw3176/Gov24Crawler-ver2/archive/refs/heads/master.zip)

2. Visual Studio를 통해 gov.sln 파일 실행
3. 프로그램 실행
    * Visual Studio 에서 실행
        * 화면 상단의 **시작** 클릭
    * EXE 파일로 실행
        1. 빌드(B) -> 일괄 빌드(T) -> Release 체크 -> 빌드(B)
        2. Gov24Crawler-ver2(-master)\gov\bin\Release 폴더의 gov.exe 실행
        
### 파일로 실행
1. [gov.zip 다운로드](https://github.com/lcw3176/Gov24Crawler-ver2/raw/master/exe/gov.zip)
2. 압축 해제 후 gov.exe 실행

## 사용법
### 설정
1. 파일 경로 설정
2. 발급 대상 주소 입력
3. 작업 범위, 지번 입력 (지목 설정 불필요)
4. 공부면적, 소유자 입력될 열 설정
#### 설정 예시
![설정 완료](https://user-images.githubusercontent.com/59993347/124360545-75bc1480-dc65-11eb-950f-2f7f125c34b9.png)
#### 입력 엑셀 파일 예시
![엑셀 파일](https://user-images.githubusercontent.com/59993347/124360564-83719a00-dc65-11eb-837f-6549e6eba853.png)
### 작업
1. ID, PW에 정부24 아이디, 비밀번호 입력
2. 시작 버튼 클릭
#### 작업 완료
![작업 완료](https://user-images.githubusercontent.com/59993347/124360560-82406d00-dc65-11eb-9fe0-b920c7665e39.png)

#### 토지 대장 캡쳐 파일 형식
![캡쳐 파일](https://user-images.githubusercontent.com/59993347/124360546-75bc1480-dc65-11eb-8d5d-d38ba9f2cf9c.png)

#### 데이터 수집된 엑셀 파일
![엑셀 결과 파일](https://user-images.githubusercontent.com/59993347/124360756-6a1d1d80-dc66-11eb-9621-260f1c6b42d5.png)


## 에러 발생 시
### 시트 이름 확인
* 데이터 수집할 페이지 이름을 Sheet1으로 변경 후 실행
### Chrome 브라우저 버전 확인
* 최신 버전이 아닐 시 작동하지 않음
### 패키지 버전 확인
* 프로젝트(P) -> Nuget 패키지 관리 로 이동 후 설치된 라이브러리 업데이트
### 파일 위치 확인
* gov.exe 파일과 chromedriver.exe, SeleniumExtras.WaitHelpers.dll, WebDriver.dll은 같은 위치에 있어야 함.