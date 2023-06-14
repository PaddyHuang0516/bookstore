using BookStore.Models.EFModels;
using BookStore.Models.Infra;
using BookStore.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace BookStore.Controllers
{
    public class MembersController : Controller
    {
        // GET: Members
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Register(RegisterVM vm)
        {
			if (ModelState.IsValid == false) return View(vm);
            //建立新會員
            Result result = RegisterMember(vm);
            //若成功，轉到ComfirmRegister
            if (result.IsSuccess)
            {
                return View("ConfirmRegister");
            }
            else
            {
                ModelState.AddModelError(string.Empty,result.ErrorMessage);
			    return View(vm);
			}
		}
        public ActionResult ActiveRegister(int memberId,string confirmCode)
        {
			//根據 memberId,confirmCode 去members找是否有一筆，有就啟用會員資格
            Result result = ActiveMember(memberId, confirmCode);  
			return View();
        }
        public ActionResult Login()
        {
            return View();
        }
		public ActionResult Logout()
		{
            Session.Abandon();
            FormsAuthentication.SignOut();
            return Redirect("/Members/Login");
		}
        public ActionResult ForgetPassword()
        {
            return View();
        }
		[HttpPost]
        public ActionResult Login(LoginVM vm)
        {
            if (ModelState.IsValid == false) return View(vm);
            //驗證帳密正確性
            Result result = ValidLogin(vm);

            if (result.IsSuccess != true)//若驗證失敗
            {
                ModelState.AddModelError (string.Empty,result.ErrorMessage);
                return View(vm);
            }
            const bool rememberMe = false;//是否記住登入會員

            //若登入帳密正確，開始處理後續登入作業，將登入帳號編碼之後加到cookie
            var processResult = ProcessLogin(vm.Account, rememberMe);
            Response.Cookies.Add(processResult.cookie);
            return Redirect(processResult.returnUrl);
        }
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }
        [Authorize]
        public ActionResult EditProfile()
        {
            var currentUserAccount = User.Identity.Name;
            var model = GetMemberProfile(currentUserAccount);
            return View(model);
        }

		private EditProfileVM GetMemberProfile(string account)
		{
			var memberInDb = new Model1().Members.FirstOrDefault(m => m.Account == account);
			return memberInDb == null
				? null
				: new EditProfileVM
				{
					Id = memberInDb.Id,
					Email = memberInDb.Email,
					Name = memberInDb.Name,
					Mobile = memberInDb.Mobile
				};
		}

		[Authorize]
        [HttpPost]
		public ActionResult EditProfile(EditProfileVM vm)
		{
            var currentUserAccount =User.Identity.Name;
            if(ModelState.IsValid==false)return View(vm);
            Result updateResult = UpdateProfile(vm);
            if (updateResult.IsSuccess) return RedirectToAction("Index");
            ModelState.AddModelError(string.Empty, updateResult.ErrorMessage);
			return View(vm);
		}

		private Result UpdateProfile(EditProfileVM vm)
		{
			// 取得在db裡的原始記錄
			var db = new Model1();

			var currentUserAccount = User.Identity.Name;
			var memberInDb = db.Members.FirstOrDefault(m => m.Account == currentUserAccount);
			if (memberInDb == null) return Result.Fail("找不到要修改的會員記錄");

			// 更新記錄
			memberInDb.Name = vm.Name;
			memberInDb.Email = vm.Email;
			memberInDb.Mobile = vm.Mobile;

			db.SaveChanges();

			return Result.Success();
		}

		[Authorize]
		public ActionResult EditPassword()
        {
            return View();
        }
        [HttpPost]
		[Authorize]
		public ActionResult EditPassword(EditPasswordVM vm)
		{
			if(ModelState.IsValid==false) return View(vm);
            var currentUserAccount = User.Identity.Name;
            Result result = ChangePassword(currentUserAccount, vm);
            if (result.IsSuccess == false)
            {
                ModelState.AddModelError(string.Empty,result.ErrorMessage);
                return View(vm);
            }
            return RedirectToAction("Index");


		}

		private Result ChangePassword(string currentUserAccount, EditPasswordVM vm)
		{
            var salt = HashUtility.GetSalt();
            var hashOriginalPassword = HashUtility.ToSHA256(vm.OriginalPassword, salt);
            var db = new Model1();
            var memberInDb = db.Members.FirstOrDefault(m=>m.Account ==currentUserAccount&&m.EncryptedPassword== hashOriginalPassword);
            if (memberInDb == null) return Result.Fail("找不到要修改的會員紀錄");
            var hashPassword = HashUtility.ToSHA256(vm.Password, salt);

            //更新密碼
            memberInDb.EncryptedPassword = hashPassword;
            db.SaveChanges();
            return Result.Success();
		}

		private (string returnUrl, HttpCookie cookie) ProcessLogin(string account, bool rememberMe)
		{
			var roles = string.Empty; // 在本範例, 沒有用到角色權限,所以存入空白

			// 建立一張認證票
			var ticket =
				new FormsAuthenticationTicket(
					1,          // 版本別, 沒特別用處
					account,
					DateTime.Now,   // 發行日
					DateTime.Now.AddDays(2), // 到期日
					rememberMe,     // 是否續存
					roles,          // userdata
					"/" // cookie位置
				);

			// 將它加密
			var value = FormsAuthentication.Encrypt(ticket);

			// 存入cookie
			var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, value);

			// 取得return url
			var url = FormsAuthentication.GetRedirectUrl(account, true); //第二個引數沒有用處

			return (url, cookie);
		}

		private Result ValidLogin(LoginVM vm)
		{
            var db = new Model1();
            var member = db.Members.FirstOrDefault(m=>m.Account==vm.Account);
            if (member == null) return Result.Fail("帳號或密碼有誤");
            if (member.IsConfirmed.HasValue == false || member.IsConfirmed.Value == false) return Result.Fail("會員資格尚未啟用");
			var salt = HashUtility.GetSalt();
			var hashPassword = HashUtility.ToSHA256(vm.Password, salt);

			return string.Compare(member.EncryptedPassword, hashPassword) == 0
				? Result.Success()
				: Result.Fail("帳密有誤");
		}

		private Result ActiveMember(int memberId,string confirmCode)
        {
            var db = new Model1();
            //根據member找出一筆紀錄 找不到就return
			//查詢方式 select * from members where isComfirmed = 0 and comfirmCode='xxx'
            var memberInDb = db.Members.FirstOrDefault(m=>m.Id == memberId &&m.IsConfirmed==false&&m.ConfirmCode == confirmCode);
            if (memberInDb == null) return Result.Success();//就算找不到也回傳成功，不要讓惡意使用者得知測試結果

            //啟用會員
			//啟用此會員的方式 update members set iscomfirmed = 1,comfirmCode = null where id =99
            memberInDb.IsConfirmed = true;
            memberInDb.ConfirmCode = null;

            db.SaveChanges();
            return Result.Success();
		}
		private Result RegisterMember(RegisterVM vm)
		{
            var db = new Model1();
            var salt = HashUtility.GetSalt();
            var hashPassword = HashUtility.ToSHA256(vm.Password, salt);

            //檢查account 是否重複
            var member = new Member
            {
                Account = vm.Account,
                Name = vm.Name,
                EncryptedPassword = hashPassword,
                Email = vm.Email,
                Mobile = vm.Mobile,
                IsConfirmed = false,
                ConfirmCode = Guid.NewGuid().ToString("N")
            };
            db.Members.Add(member);
            db.SaveChanges();

            //todo 寄發 email
            return Result.Success();
		}
	}
}