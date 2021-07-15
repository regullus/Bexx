"use strict";

// Class Definition
var KTLogin = function () {
    var _login;

    var _showForm = function (form) {
        var cls = 'login-' + form + '-on';
        var form = 'kt_login_' + form + '_form';

        _login.removeClass('login-forgot-on');
        _login.removeClass('login-signin-on');
        _login.removeClass('login-signup-on');

        _login.addClass(cls);

        KTUtil.animateClass(KTUtil.getById(form), 'animate__animated animate__backInUp');
    }

    var _handleSignInForm = function () {
        // Handle forgot button
        $('#kt_login_forgot').on('click', function (e) {
            e.preventDefault();
            _showForm('forgot');
        });

        // Handle signup
        $('#kt_login_signup').on('click', function (e) {
            e.preventDefault();
            _showForm('signup');
        });
    }

    var _handleSignUpForm = function (e) {
        var validation;
        var form = KTUtil.getById('kt_login_signup_form');

        // Init form validation rules. For more info check the FormValidation plugin's official documentation:https://formvalidation.io/
        validation = FormValidation.formValidation(
            form,
            {
                fields: {
                    name: {
                        validators: {
                            notEmpty: {
                                message: 'O nome do \u00fasuario \u00e9 necess\u00e1rio'
                            }
                        }
                    },
                    email: {
                        validators: {
                            notEmpty: {
                                message: 'O email \u00e9 necess\u00e1rio'
                            },
                            emailAddress: {
                                message: 'O Email informado n\u00e3o \u00e9 v\u00e1lido'
                            }
                        }
                    },
                    login: {
                        validators: {
                            notEmpty: {
                                message: 'O login \u00e9 necess\u00e1rio'
                            },
                        }
                    },
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
                    agree: {
                        validators: {
                            notEmpty: {
                                message: 'Voc\u00ea deve aceitar os termos e condi\u00e7\u00f5es'
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

        $('#kt_login_signup_submit').on('click', function (e) {
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
                    $.post(pathCadastro, {
                        nome: $('#CadNome').val(),
                        email: $('#CadEmail').val(),
                        login: $('#CadLogin').val(),
                        password: $('#CadPassword').val(),
                        confirmarPassword: $('#CadConfirmarPassword').val(),
                        aceite: $('#CadAceite').val(),
                        token: $('#tokenCadastro').val(),
                    }, function (data) {
                        KTApp.unblock('#login-form');
                        if (data == 'ok') {
                            swal.fire({
                                text: "Dados cadastrados com sucesso! Verifique seu email para validar seu cadastro.",
                                icon: "success",
                                buttonsStyling: false,
                                confirmButtonText: "OK",
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
                                html: "Desculpe, n\u00e3o foi poss\u00edvel efetuar o cadastro, motivo: <br /><br/><span style='color: #FF3333;'>" + data + "</ span>",
                                icon: "error",
                                buttonsStyling: false,
                                confirmButtonText: "OK",
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
                        confirmButtonText: "OK",
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
        $('#kt_login_signup_cancel').on('click', function (e) {
            e.preventDefault();
            _showForm('signin');
        });
    }

    var _handleForgotForm = function (e) {
        var validation;
        var form = KTUtil.getById('kt_login_forgot_form');

        // Init form validation rules. For more info check the FormValidation plugin's official documentation:https://formvalidation.io/
        validation = FormValidation.formValidation(
            form,
            {
                fields: {
                    login: {
                        validators: {
                            notEmpty: {
                                message: 'O login \u00e9 necess\u00e1rio'
                            },
                        }
                    },
                },
                plugins: {
                    trigger: new FormValidation.plugins.Trigger(),
                    bootstrap: new FormValidation.plugins.Bootstrap()
                }
            }
        );

        // Handle submit button
        $('#kt_login_forgot_submit').on('click', function (e) {
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
                    $.post(pathEsqueceuSenha , {
                        email: '',
                        login: $('#EsqLogin').val(),
                        password: '',
                        token: $('#tokenEsqueci').val(),
                    }, function (data) {
                        KTApp.unblock('#login-form');
                        if (data == 'ok') {
                            swal.fire({
                                text: "Instru\u00e7\u00f5es enviadas! Verifique seu email para dar continuidade a recuper\u00e7\u00e3o de sua senha.",
                                icon: "success",
                                buttonsStyling: false,
                                confirmButtonText: "OK",
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
                                html: "Desculpe, n\u00e3o foi poss\u00edvel enviar as instru\u00e7\u00f5es para recuperar a senha, motivo: <br /><br/><span style='color: #FF3333;'>" + data + "</ span>",
                                icon: "error",
                                buttonsStyling: false,
                                confirmButtonText: "OK",
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
                        text: "Desculpe!, parece que foram detectados alguns erros, tente novamente...",
                        icon: "error",
                        buttonsStyling: false,
                        confirmButtonText: "OK",
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
        $('#kt_login_forgot_cancel').on('click', function (e) {
            e.preventDefault();
            grecaptcha.ready(function () {
                grecaptcha.execute(keyReCaptcha, { action: 'homepage' }).then(function (token) {
                    $('#token').val(token);
                    $('#tokenCadastro').val(token);
                    $('#tokenEsqueci').val(token);
                });
            });
            _showForm('signin');
        });
    }

    // Public Functions
    return {
        // public functions
        init: function () {
            _login = $('#kt_login');
            _handleSignInForm();
            _handleSignUpForm();
            _handleForgotForm();
        }
    };
}();

// Class Initialization
jQuery(document).ready(function () {
    KTLogin.init();
});
