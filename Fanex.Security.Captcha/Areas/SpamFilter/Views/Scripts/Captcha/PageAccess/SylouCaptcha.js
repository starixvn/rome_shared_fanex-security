var googleRecaptcha
jQuery(document).ready(function () {
    googleRecaptcha = new GoogleRecaptcha();
    googleRecaptcha.Init();
    if (jQuery('#ValidateSuccess').val() == "True") {
        googleRecaptcha.ClosePopup();
    }
    else {
        googleRecaptcha.ShowPopup();
    }
});



var GoogleRecaptcha = function () {
    var self = this;
    var _elements =
    {
        maskedWindow: null,
        txtPageName: null
    };

    self.Init = function() {
        _elements.maskedWindow = jQuery('#frCaptcha', window.parent.document);
    };

    self.ShowPopup = function () {
        _elements.maskedWindow.show();
    };

    self.ClosePopup = function () {
        try {
           _elements.maskedWindow.remove();
        } catch (e) {

        }

    };
}