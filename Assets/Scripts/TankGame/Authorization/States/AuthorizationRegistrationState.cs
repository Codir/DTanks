﻿using Networking.Msg;
using TankGame.Forms;

namespace TankGame.Authorization.States
{
    public class AuthorizationRegistrationState : AuthorizationFormStateBase<RegisterForm>
    {
        public override void Start(object[] args = null)
        {
            base.Start(args);
            FormView.SendButton.onClick.AddListener(OnSignUpClicked);
            FormView.ToLogInButton.onClick.AddListener(OnSignInClicked);
        }

        public override void Finish()
        {
            base.Finish();
            FormView.SendButton.onClick.RemoveListener(OnSignUpClicked);
            FormView.ToLogInButton.onClick.RemoveListener(OnSignInClicked);
        }

        private void OnSignUpClicked()
        {
            FormView.ShowWaitingForm();

            var message = new RegistrationMessage
            {
                UserName = FormView.LoginField.text,
                Password = FormView.PasswordField.text
            };
            Client.Send(message);
        }

        protected override void OnRegistrationCallback(RegistrationAnswerMessage message)
        {
            base.OnRegistrationCallback(message);
            FormView.HideWaitingForm();
            if (message.Error != NetMsgErrorType.None)
            {
                FormView.ShowError(NetMsgErrorMessages.GetMessage(message.Error));
            }
            else
            {
                Model.UserName = FormView.LoginField.text;
                Model.Password = FormView.PasswordField.text;
                Model.SetChanges();
                ApplyState<AuthorizationSuccessRegistrationState>();
            }
        }

        private void OnSignInClicked()
        {
            ApplyState<AuthorizationLoginState>();
        }
    }
}