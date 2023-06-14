[v] add EFModels
		-add Models/Infra/HashUtilities.cs/EmailHelper
		-修改web-config websetting 放入salt,value

[v] add RegisterVM.cs 註冊使用的vm
		--add MembersController,Register actions
		-add Infra/Result.cs
		-修改_Layout vies page,加入'註冊新會員' nav item

[v] 實作 新會員 確認功能
		-正式啟用會員資格, url template = /members/activeRegister?memberId=99&confirmCode=xxx
		-MemberController add actions:
			-(bool,string) Load(memberId)
			-ActiveResgister(memberId,comfirmCode)
			-ActiveRegister view page

[v] 實作 登入/登出網站
		- 只有帳密正確而且已正式開通的會員才允許登入, 實作之前, 請先各別建立一個已/未開通的會員記錄,方便測試
		- modify web.config, add Authenthcation node
		- add LoginVM
		- add MemberController.Login() actions, Login.cshtml
		- modify MemberController.About, add "Authorize" attribute, 若沒登入過,會自動導向到/Members/Login
		- modify MembersController, add HttpPost Logout action
		- modify _Layout page, add "Login/Logout" links
		- todo 需要加做 Members/Index, 會員中心頁, 在登入成功之後會導向到此頁
		- 驗證: 目前在沒登入時,會自動判斷權限, 無法檢視 About page; 登入/登出功能已實作

[V]實作 修改個人基本資料-建立會員中心頁
		- modify MembersController, add Index action
		- add Views/Members/Index.cshtml(空白範本), 填入二個超連結 : ""修改個人基本資料", "重設密碼"

[V]實作 修改個人基本資料
	- add EditProfileVM.cs(作為稍後 EditProfile.cshtml 的model,由於不允許修改帳號,所以這類別裡沒有Account)
	- add MemberExts class, 擴充方法 ToEditProfileVm(Member)
	- modify MembersController, add EditProfile actions, 要加Authorize
	- add "EditProfile" view page(使用 Edit 範本)	

[]實作 變更密碼
	- EditPasswordVM.cs
	- modify MembersController, add EditPassword actions, call UpdatePassword(int memberId, string newEncryptedPassword)
	- add EditPassword view page(用create 範本)