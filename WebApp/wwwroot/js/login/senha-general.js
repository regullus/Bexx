"use strict";

// Class Definition
var KTLogin = function() {
    var _login;

    var _handleNovaSenhaForm = function (e) {
        var validation;
        var form = KTUtil.getById('kt_nova_senha_form');

        // Init form validation rules. For more info check the FormValidation plugin's official documentation:https://formvalidation.io/
        validation = FormValidation.formValidation(
            form,
            {
                fields: {
                    password: {
                        validators: {
                            notEmpty: {
                                message: 'A senha \u00e9 necess\u00e1ria'
                            }
                        }
                    },
                    cpassword: {
                        validators: {
                            notEmpty: {
                                message: 'A senha de confirma\u00e7\u00e3o \u00e9 necess\u00e1ria'
                            },
                            identical: {
                                compare: function () {
                                    return form.querySelector('[name="password"]').value;
                                },
                                message: 'A  <span style="color: #ff3333; font-weight:bold;">senha</span> e a <span style="color: #ff3333; font-weight:bold;">senha de confirma\u00e7\u00e3o</span> devem ser iguais.'
                            }
                        }
                    },
                },
                plugins: {
                    trigger: new FormValidation.plugins.Trigger(),
                    bootstrap: new FormValidation.plugins.Bootstrap()
                }
            }
        );

        $('#kt_nova_senha_submit').on('click', function (e) {
            e.preventDefault();
            validation.validate().then(function (status) {
                if (status == 'Valid') {
                    KTApp.block('#login-form', {
                        opacity: 0.5,
                        overlayColor: '#000000',
                        state: 'danger', // a bootstrap color
                        size: 'lg', //available custom sizes: sm|lg
                        message: 'Aguarde...'
                    });
                    $.post(pathCriarSenha, {
                        password: $('#NovPassword').val(),
                        confirmarPassword: $('#NovConfirmarPassword').val(),
                        token: $('#token').val(),
                        token2: $('#tokenNovaSenha').val(),
                    }, function (data) {
                        KTApp.unblock('#login-form');
                        if (data == 'ok') {
                            swal.fire({
                                text: "Senha cadastrada com sucesso!",
                                icon: "success",
                                buttonsStyling: false,
                                confirmButtonText: "Ok",
                                customClass: {
                                    confirmButton: "btn font-weight-bold btn-light-primary"
                                }
                            }).then(function () {
                                grecaptcha.ready(function () {
                                    grecaptcha.execute(keyReCaptcha, { action: 'homepage' }).then(function (token) {
                                        $('#token').val(token);
                                        $('#tokenCadastro').val(token);
                                        $('#tokenEsqueci').val(token);
                                    });
                                });
                                $(window.location).attr('href', pathLogin);
                            });
                        }
                        else {
                            swal.fire({
                                html: "Desculpe, n\u00e3o foi poss\u00edvel efetuar o cadastro da senha, motivo: <br/><br/><span style='color: #FF3333;'>" + data + "</span>",
                                icon: "error",
                                buttonsStyling: false,
                                confirmButtonText: "Ok!",
                                customClass: {
                                    confirmButton: "btn font-weight-bold btn-light-primary"
                                }
                            }).then(function () {
                                grecaptcha.ready(function () {
                                    grecaptcha.execute(keyReCaptcha, { action: 'homepage' }).then(function (token) {
                                        $('#token').val(token);
                                        $('#tokenCadastro').val(token);
                                        $('#tokenEsqueci').val(token);
                                    });
                                });
                                KTUtil.scrollTop();
                            });
                        }
                    });
                } else {
                    swal.fire({
                        text: "Desculpe, parece que foram detectados alguns erros, tente novamente...",
                        icon: "error",
                        buttonsStyling: false,
                        confirmButtonText: "Ok!",
                        customClass: {
                            confirmButton: "btn font-weight-bold btn-light-primary"
                        }
                    }).then(function () {
                        grecaptcha.ready(function () {
                            grecaptcha.execute(keyReCaptcha, { action: 'homepage' }).then(function (token) {
                                $('#token').val(token);
                                $('#tokenCadastro').val(token);
                                $('#tokenEsqueci').val(token);
                            });
                        });
                        KTUtil.scrollTop();
                    });
                }
            });
        });

        // Handle cancel button
        $('#kt_nova_senha_cancel').on('click', function (e) {
            e.preventDefault();
            grecaptcha.ready(function () {
                grecaptcha.execute(keyReCaptcha, { action: 'homepage' }).then(function (token) {
                    $('#token').val(token);
                    $('#tokenCadastro').val(token);
                    $('#tokenEsqueci').val(token);
                });
            });
            $(window.location).attr('href', pathLogin);
        });
    }

    // Public Functions
    return {
        // public functions
        init: function () {
            _handleNovaSenhaForm();
        }
    };

}();

// Class Initialization
jQuery(document).ready(function() {
    KTLogin.init();
});
