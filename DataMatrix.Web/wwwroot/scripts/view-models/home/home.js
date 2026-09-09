import Vue from 'vue/dist/vue.esm.js';

import { getModel } from '../../main.js';
import getImage from '../../services/image-service.js';

import textbox from '../../../components/textbox/textbox.vue';
import pager from '../../../components/pager/pager.vue';
import popup from '../../../components/popup/popup.vue';
import popupOpener from '../../../components/popup/popup-opener.vue';
import modalWindow from '../../../components/modal/modal-window.vue';

import axios from 'axios';

document.addEventListener('DOMContentLoaded',
    function () {
        const data = getModel();
        data.isRegister = false;
        data.login = '';
        data.password = '';
        data.checkedRoles = [];
        data.loginIsValid = true;
        data.passwordIsValid = true;
        data.errorMessage = '';
        data.successMessage = '';
        data.image = '';

        const vs = new Vue({
            el: '#global-wrapper',
            data: data,
            components: { textbox, pager, popup, popupOpener, modalWindow },
            beforeDestroy: function () {
                getImage.destroy();
            },
            mounted: function () {
                this.clearCheckedRoles();
                if (this.isAuthorized) {
                    this.goToPage();
                }
            },
            computed: {
                loginButtonIsEnabled() {
                    var data = this;
                    return data.login.length > 0 &&
                        data.password.length > 0 &&
                        data.loginIsValid &&
                        data.passwordIsValid;
                },
                registerButtonIsEnabled() {
                    var data = this;
                    return data.checkedRoles.filter(role => role.isChecked).length > 0 &&
                        data.login.length > 0 &&
                        data.password.length > 0 &&
                        data.loginIsValid &&
                        data.passwordIsValid;
                }
            },
            methods: {
                clearCheckedRoles() {
                    var data = this;
                    data.checkedRoles = Object.entries(data.roles).map(([key, value]) => ({
                        id: Number(key),
                        title: value,
                        isChecked: false
                    }));
                },
                logout() {
                    var data = this;
                    axios({
                        url: `/home/logout`,
                        method: 'post'
                    }).then(function () {
                        data.login = '';
                        data.password = '';
                        data.isRegister = false;
                        data.isAuthorized = false;
                        data.clearCheckedRoles();
                        data.showSuccessMessage("Вы успешно вышли из системы.");
                    }).catch(function (error) {
                        console.error(error);
                        data.showErrorMessage("Неизвестная ошибка во время выхода из системы.");
                    });
                },
                submitLogin() {
                    var data = this;
                    axios({
                        url: `/home/auth`,
                        method: 'post',
                        data: { login: data.login, password: data.password }
                    }).then(function () {
                        data.isRegister = false;
                        data.isAuthorized = true;
                        data.goToPage();
                    }).catch(function (error) {
                        data.isAuthorized = false;
                        console.error(error);
                        if (error.response && error.response.status === 401) {
                            data.showErrorMessage('Неверный логин или пароль');
                        }
                        else {
                            data.showErrorMessage('Ошибка при авторизации.');
                        }
                    });
                },
                submitRegister() {
                    var data = this;
                    var selectedRoles = data.checkedRoles.filter(role => role.isChecked).map(role => role.id);
                    axios({
                        url: `/home/register`,
                        method: 'post',
                        data: { login: data.login, password: data.password, roles: selectedRoles }
                    }).then(function () {
                        data.isRegister = false;
                        data.isAuthorized = true;
                        data.goToPage();
                    }).catch(function (error) {
                        console.error(error);
                        data.showErrorMessage('Неизвестная ошибка при регистрации.');
                        data.isAuthorized = false;
                    });
                },
                createImage() {
                    var data = this;
                    axios({
                        url: `/home/create`,
                        method: 'post'
                    }).then(function () {
                        data.showSuccessMessage('Код успешно создан');
                        data.goToPage();
                    }).catch(function (error) {
                        console.error(error);
                        if (error.response) {
                            if (error.response.status === 401) {

                                if (error.response.message === 'relogin') {
                                    data.login = '';
                                    data.password = '';
                                    data.isAuthorized = false;
                                }
                            }
                            else if (error.response.status === 403) {
                                data.showErrorMessage("Недостаточно прав для создания картинки.");
                            }
                            else {
                                data.showErrorMessage('Неизвестная ошибка');
                            }
                        }
                        else {
                            data.showErrorMessage('Неизвестная ошибка');
                        }
                    });
                },
                async setImage(id) {
                    var data = this;
                    try {
                        data.image = await getImage.open(id);
                        data.$refs['code-modal'].showed = true;
                    }
                    catch (error) {
                        console.error(error);
                        if (error.response) {
                            if (error.response.status === 401) {

                                if (error.response.message === 'relogin') {
                                    data.login = '';
                                    data.password = '';
                                    data.isAuthorized = false;
                                }
                            }
                            else if (error.response.status === 404) {
                                data.showErrorMessage("Не найдено изображения по указанному ID.");
                            }
                            else if (error.response.status === 403) {
                                data.showErrorMessage("Недостаточно прав для скачивания картинки.");
                            }
                            else {
                                data.showErrorMessage('Неизвестная ошибка');
                            }
                        }
                        else {
                            data.showErrorMessage('Неизвестная ошибка');
                        }
                    }
                },
                downloadImage() {
                    var data = this;
                    if (!data.image) {
                        data.showErrorMessage('Попытка скачать несуществующую картинку');
                        return;
                    }
                    const link = document.createElement('a');
                    link.href = data.image;
                    link.download = 'code.png';
                    link.click();
                },
                closeImageModal() {
                    var data = this;
                    data.$refs['code-modal'].showed = false;
                    data.image = '';
                },
                goToPage(pageNumber = 1) {
                    var data = this;
                    if (pageNumber <= 0 || (data.totalPages != 0 && pageNumber > data.totalPages)) {
                        data.showErrorMessage('Попытка открыть несуществующую страницу');
                        return;
                    }
                    axios({
                        url: `/home/codes/${pageNumber}`,
                        method: 'get'
                    }).then(function (response) {
                        var res = response.data;
                        data.currentPage = res.currentPage;
                        data.pageSize = res.pageSize;
                        data.totalPages = res.totalPages;
                        data.codes = res.codes;
                    }).catch(function (error) {
                        console.error(error);
                        if (error.response) {
                            if (error.response.status === 401) {

                                if (error.response.message === 'relogin') {
                                    data.login = '';
                                    data.password = '';
                                    data.isAuthorized = false;
                                }
                            }
                            else if (error.response.status === 403) {
                                data.showErrorMessage("Недостаточно прав для получения кодов.");
                            }
                            else {
                                data.showErrorMessage('Неизвестная ошибка');
                            }
                        }
                        else {
                            data.showErrorMessage('Неизвестная ошибка');
                        }
                    });

                    console.log(data);
                },
                loginRules() {
                    return [
                        v => !!v || "Пожалуйста, введите логин",
                        v => !!/^[A-Za-z0-9]*$/.exec(v) || "Пожалуйста, введите корректный логин",
                        v => v.length <= 50 || "Логин слишком длинный",
                        v => v.length >= 6 || "Логин слишком короткий"
                    ];
                },
                passwordRules() {
                    return [
                        v => !!v || "Пожалуйста, введите пароль",
                        v => !!/[A-Z]/.exec(v) || "Пароль должен содержать хотя бы одну заглавную букву",
                        v => !!/[a-z]/.exec(v) || "Пароль должен содержать хотя бы одну строчную букву",
                        v => !!/[0-9]/.exec(v) || "Пароль должен содержать хотя бы одну цифру",
                        v => !!/[!@#$%^&*(),.?":{}|<>]/.exec(v) || "Пароль должен содержать хотя бы один специальный символ",
                        v => v.length >= 6 || "Пароль слишком коротки",
                        v => v.length <= 50 || "Пароль слишком длинный"
                    ]
                },
                moveToRegister() {
                    this.isRegister = true;
                },
                cancelRegister() {
                    this.isRegister = false;
                },
                showErrorMessage(message) {
                    var data = this;
                    data.$refs['error-modal'].showed = true;
                    data.errorMessage = message;
                },
                showSuccessMessage(message) {
                    var data = this;
                    data.$refs['success-modal'].showed = true;
                    data.successMessage = message;
                },
                closeErrorModal() {
                    var data = this;
                    data.$refs['error-modal'].showed = false;
                    data.errorMessage = '';
                },
                closeSuccessModal() {
                    var data = this;
                    data.$refs['success-modal'].showed = false;
                    data.successMessage = '';
                }
            }
        })
    })