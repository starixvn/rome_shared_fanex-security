var googleRecaptcha
jQuery(document).ready(function () {
    googleRecaptcha = new GoogleRecaptcha();
    googleRecaptcha.Init();
    googleRecaptcha.ShowPopup();
});



var GoogleRecaptcha = function () {
    var self = this;
    var _elements =
    {
        mainPage: null,
        maskedWindow: null,
        menu: null,
        popupWindow: null,
        overlay: null,
        btnSubmit: null,
        txtChallengeField: null,
        txtResponseField: null,
        errMsg: null,
        txtPageName: null
    };

    self.Init = function () {
        _elements.maskedWindow = jQuery('#captchaWrapper');
        _elements.popupWindow = jQuery("#popup");
        _elements.overlay = jQuery('#overlay');
        _elements.btnSubmit = jQuery("#SubmitCaptcha");
        _elements.txtChallengeField = jQuery("#recaptcha_challenge_field")
        _elements.txtResponseField = jQuery("#recaptcha_response_field");
        _elements.errMsg = jQuery("#message-box");
        _elements.maskedWindow.hide();
        bindingEvents();
    }

    var bindingEvents = function () {
        _elements.btnSubmit.click(function () {
            jQuery.ajax({
                type: "POST",
                url: window.parent.Captcha.RootURL + 'SpamFilter/Captcha/CheckCaptcha',
                data: "recaptcha_challenge_field=" + _elements.txtChallengeField.val() + "&recaptcha_response_field=" + _elements.txtResponseField.val() + "&captchaValid=&pageName=" + window.parent.Captcha.PageName,
                dataType: "json",
                success: function (rs) {
                    if (rs) {
                        _elements.errMsg.hide();
                        self.ClosePopup();
                    }
                    else {
                        Recaptcha.reload();
                        _elements.errMsg.show();
                    }
                }
            });
        });
    }

    self.ShowPopup = function () {
        _elements.maskedWindow.show();
    };

    self.ClosePopup = function () {
        _elements.maskedWindow.hide();
    };
}