using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VehiclesControl.Domain.Outs
{
    public class ResultCode
    {
        private static ResultCode _Instance;

        public static ResultCode Instance
        {
            get
            {
                if (_Instance == null)
                {
                    SetSingleton(new ResultCode());
                }

                return _Instance;
            }
        }

        public int Unauthorized { get; protected set; }

        public int Ok { get; protected set; }

        public int Failed { get; protected set; }

        public int NotSupported { get; protected set; }

        public int NotFound { get; protected set; }

        public int InvalidCast { get; protected set; }

        public int Locked { get; protected set; }

        public int InvalidConcurrencyStamp { get; protected set; }

        public int DatabaseSaveFailed { get; protected set; }

        public int Duplicate { get; protected set; }

        public int LoginNotFound { get; protected set; }

        public int LoginInvalid { get; protected set; }

        public int LoginTokenFailed { get; protected set; }

        public int LoginSuspended { get; protected set; }

        public int InvalidPassword { get; protected set; }

        public int ResetExpired { get; protected set; }

        public int ResetUsed { get; protected set; }

        public int PasswordMinimum { get; protected set; }

        public int PasswordMaximum { get; protected set; }

        public int PasswordNumeric { get; protected set; }

        public int PasswordAlphaNumeric { get; protected set; }

        public int PasswordSpecialChars { get; protected set; }

        public int PasswordLowerCase { get; protected set; }

        public int PasswordUpperCase { get; protected set; }


        public static bool SetSingleton(ResultCode instance)
        {
            if (_Instance != null)
            {
                return false;
            }

            _Instance = instance;
            instance.Load();
            return true;
        }

        public virtual void Load()
        {
            LoginNotFound = 210;
            LoginInvalid = 211;
            LoginTokenFailed = 212;
            LoginSuspended = 213;
            InvalidPassword = 220;
            ResetExpired = 221;
            ResetUsed = 222;
            PasswordMinimum = 230;
            PasswordMaximum = 231;
            PasswordNumeric = 232;
            PasswordAlphaNumeric = 233;
            PasswordSpecialChars = 234;
            PasswordLowerCase = 235;
            PasswordUpperCase = 236;
            Unauthorized = 403;
            Ok = 200;
            Failed = 500;
            NotSupported = 450;
            NotFound = 404;
            InvalidCast = 501;
            Locked = 550;
            InvalidConcurrencyStamp = 551;
            DatabaseSaveFailed = 552;
            Duplicate = 553;
        }
    }
}
