import validationError from '../validation/validation-error.vue';
import passwordShow from '../password-show/password-show.vue';

export default {
    components: {
        validationError,
        passwordShow
    },
    data: function () {
        return {
            focused: false,
            internalValue: this.value === null ? '' : this.value,
            isValid: true,
            isWarning: false,
            showPassword: false,
            bluredOnceOrMore: false,
        };
    },
    props: {
        name: {
            type: String,
            default: function () {
                return undefined;
            }
        },
        transform: {
            type: Function,
            default: null
        },
        placeholder: {
            type: String,
            required: false
        },
        label: {
            type: String
        },
        value: {
            type: String,
            default: ''
        },
        icon: {
            type: String,
            default: null
        },
        validationMode: {
            type: String,
            default: 'label',
            validator: function (value) {
                return ['label', 'icon'].indexOf(value) !== -1;
            }
        },
        validateOnBlur: {
            type: Boolean,
            default: false
        },
        rules: {
            type: Array,
            default: function () { return []; }
        },
        inputMode: {
            type: String,
            default: 'text',
            validator: function (value) {
                return ['text', 'phone', 'date', 'password', 'number'].indexOf(value) !== -1;
            }
        },
        allowAutoCompletion: {
            type: Boolean,
            default: false
        },
        allowClipboardInsertion: {
            type: Boolean,
            default: true
        },
        maxNumber: {
            type: [Number, String],
            default: null
        },
        maxLength: {
            type: [Number, String],
            default: null
        },
        validSymbols: {
            type: [String],
            default: ''
        },
        readonly: {
            type: Boolean,
            default: false
        },
        disabled: {
            type: Boolean,
            default: false
        },
        showPasswordToggle: {
            type: Boolean,
            default: false
        }
    },
    mounted() {
        let input = this.$refs[this.inputName];
        window.addEventListener("pageshow", () => {
            input.value = this.value;
        });
    },
    computed: {
        visiblePlaceholder() {
            if (!this.focused)
                return "";
            else
                return this.placeholder;
        },
        hasValue: function () {
            return this.internalValue.trim() !== '';
        },
        showValidationIcon: function () {
            return this.validationMode === 'icon' && !this.isValid;
        },
        inputType: function () {
            if (this.inputMode === 'date')
                return 'date';
            if (this.inputMode === 'password')
                return this.showPassword ? 'text' : 'password';
            if (this.inputMode === 'number')
                return 'number';
            return 'text';
        },
        autocomplete: function () {
            return this.allowAutoCompletion ? 'on' : 'off';
        },
        inputName: function () {
            return this.name + '-input';
        }
    },
    watch: {
        value: function (newValue) {
            this.internalValue = newValue === null ? '' : newValue;
        },
        internalValue(newValue) {
            this.validate(newValue);
        }
    },
    methods: {
        validate() {
            const vm = this;

            if (!vm.bluredOnceOrMore) {
                return;
            }

            if (!!this.rules) {
                for (const rule of this.rules) {
                    var result = rule(vm.internalValue);
                    var validation = vm.$refs[this.inputName + '-validation'];
                    if (!validation)
                        return;

                    var message;
                    if (typeof (result) === "string") {
                        message = {
                            isValid: false,
                            error: result,
                            isWarning: false
                        };
                        validation.validate(message);
                        vm.handleValidChange(message);
                        break;
                    } else if (typeof (result) === "boolean") {
                        message = {
                            isValid: result,
                            error: null,
                            isWarning: false
                        };
                        validation.validate(message);
                        vm.handleValidChange(message);
                    }
                }
            }
        },
        handleFocus: function () {
            this.focused = true;
            this.$emit('focus', event);
        },
        handleBlur: function (event) {
            this.focused = false;
            this.$emit('blur', event);
            this.bluredOnceOrMore = true;

            if (this.validateOnBlur)
                this.validate();
        },
        handlePaste: function (event) {
            if (this.allowClipboardInsertion === false) {
                event.returnValue = false;
                event.preventDefault();
                return;
            }

            let clipboardData = event.clipboardData.getData('text/plain');
            if (this.validSymbols.length > 0)
                clipboardData = clipboardData.split('').filter(ch => this.validSymbols.split('').includes(ch)).join('');

            if (this.inputMode === 'number') {

                var regex = /[^\d]/g;
                clipboardData = clipboardData.replace(regex, '');
                if (this.maxLength)
                    clipboardData = clipboardData.substring(0, Number(this.maxLength));
                event.clipboardData.setData('text/plain', clipboardData);
            }

            if (this.maxLength && clipboardData && clipboardData.length > Number(this.maxLength)) {
                clipboardData = clipboardData.substring(0, Number(this.maxLength));
                event.clipboardData.setData('text/plain', clipboardData);
            }

            if (clipboardData !== event.clipboardData.getData('text/plain'))
                event.clipboardData.setData('text/plain', clipboardData);
        },
        handleKeypress: function (event) {
            if (this.inputMode !== 'number' && this.inputMode !== 'date')
                return;

            if (event === null || event === undefined)
                event = window.event;

            if (event === null || event === undefined)
                return;

            let key;

            if (event.type === 'paste') {
                key = event.clipboardData.getData('text/plain');

                var regex = /[0-9]/;
                if (!regex.test(key)) {
                    event.returnValue = false;
                    if (event.preventDefault)
                        event.preventDefault();
                }
            } else {
                key = event.keyCode || event.which;
                key = String.fromCharCode(key);
            }

            if (key === ',' || key === '.') {
                event.returnValue = false;
                if (event.preventDefault)
                    event.preventDefault();
            }

            if (this.validSymbols.length > 0 && !this.validSymbols.split('').includes(key)) {
                event.returnValue = false;
                if (event.preventDefault)
                    event.preventDefault();
            }
        },
        handleKeydown: function (event) {

            if (this.inputMode === 'text' || this.inputMode === 'password' || this.inputMode === 'date')
                return;

            var keyCode = event.keyCode;
            if (event.ctrlKey === false
                && ((keyCode >= 65 && keyCode <= 90)
                    || (keyCode <= 106 && keyCode >= 107)
                    || (keyCode <= 110 && keyCode >= 111)
                    || (keyCode <= 186 && keyCode >= 222)
                    || (keyCode == 187 && event.shiftKey === false))) {
                event.preventDefault();
                return;
            }

            if (this.inputMode === 'number' && event.ctrlKey === false && (keyCode == 187 || keyCode == 189 || keyCode == 109 || keyCode == 107)) {
                event.preventDefault();
                return;
            }

            var isDeleteKey = (keyCode === 46);
            var isBackspaceKey = (keyCode === 8);
            var currentValue = event.target.value;
            var caretIndex = event.target.selectionStart;

            var handleCharacterDelete = function (component, isBackward) {

                var firstPart = currentValue.slice(0, isBackward ? event.target.selectionStart - 2 : event.target.selectionStart);
                var secondPart = currentValue.slice(isBackward ? event.target.selectionStart : event.target.selectionStart + 2);

                if (isBackward) {
                    firstPart += secondPart.slice(0, 1);
                    secondPart = secondPart.slice(1);
                }

                var parts = secondPart.split('-');
                for (var i = 1; i < parts.length; i++) {
                    var firstChar = parts[i].charAt(0);
                    parts[i - 1] = parts[i - 1] + firstChar;
                    parts[i] = parts[i].slice(1);
                }

                var value = firstPart;
                if (parts.filter(part => part.length > 0).length > 0)
                    value += '-' + parts.join('-');

                event.target.value = value;
                component.internalValue = event.target.value;
            }

            var removedSymbol;
            if (isBackspaceKey && caretIndex > 0) {
                if (caretIndex <= 3) {
                    event.preventDefault();
                    return;
                }

                removedSymbol = currentValue.charAt(caretIndex - 1);
                if (removedSymbol === '-') {
                    handleCharacterDelete(this, true);

                    event.preventDefault();
                    event.target.setSelectionRange(caretIndex - 2, caretIndex - 2);
                    this.$emit('input', event.target.value);
                }
            }
            else if (isDeleteKey && caretIndex <= currentValue.length) {
                if (caretIndex < 2) {
                    caretIndex = 2;
                    event.target.selectionStart = 2;
                }

                removedSymbol = currentValue.charAt(caretIndex);
                if (removedSymbol === '-') {
                    handleCharacterDelete(this, false);

                    event.preventDefault();
                    event.target.setSelectionRange(caretIndex + 1, caretIndex + 1);
                    this.$emit('input', event.target.value);
                }
            }
        },
        handleInput: function (event) {

            if (this.validSymbols.length > 0)
                event.target.value = event.target.value.split('').filter(ch => this.validSymbols.split('').includes(ch)).join('');

            if (this.maxLength && event.target.value && event.target.value.length > Number(this.maxLength)) {
                let trimmed = event.target.value.substring(0, Number(this.maxLength));
                event.target.value = this.internalValue;
                event.preventDefault();
                this.$emit('input', trimmed);
                return;
            }

            if (event.inputType === 'insertFromPaste' && !this.allowClipboardInsertion) {
                event.target.value = this.internalValue;
                event.preventDefault();
                return;
            }

            if (this.inputMode === 'number') {
                let number = new Number(event.target.value);
                if ((number > this.maxNumber && this.maxNumber !== null && this.maxNumber !== undefined)) {
                    event.target.value = this.maxNumber === null || this.maxNumber === undefined ? this.internalValue : this.maxNumber;
                    event.preventDefault();
                    this.$emit('input', event.target.value);
                    return;
                }
            }
            if (this.inputMode === 'text' || this.inputMode === 'password' || this.inputMode === 'number' || this.inputMode === 'date')
                this.internalValue = event.target.value;
            else if (this.inputMode === 'phone') {

                if (event.inputType === undefined || event.inputType === 'insertText') {

                    var data;

                    if (event.data === undefined) {
                        if (this.internalValue.length > event.target.value.length) {
                            this.internalValue = event.target.value;
                            return;
                        }

                        data = event.target.value[event.target.selectionStart - 1];
                    } else {
                        data = event.data;
                    }
                    var maskedPhone = this.getMaskedPhone(this.internalValue, event.target.selectionStart, data);
                    this.internalValue = maskedPhone.value;
                    setTimeout(() => event.target.setSelectionRange(maskedPhone.caretIndex, maskedPhone.caretIndex), 0);
                } else {
                    var caretIndex = event.target.selectionStart;
                    this.internalValue = this.internalValue.slice(0, caretIndex) + this.applyPhoneMask(this.internalValue.slice(caretIndex + 1), caretIndex);
                    setTimeout(() => event.target.setSelectionRange(caretIndex, caretIndex), 0);
                }
            }

            if (!!this.transform) {
                event.target.value = this.transform(event.target.value);
                this.internalValue = this.transform(event.target.value);
            }

            this.$emit('input', event.target.value);
        },
        handleValidChange: function (validationMessage) {
            this.isValid = validationMessage.isValid;
            this.isWarning = validationMessage.isWarning;
            this.$emit('valid-change', this.isValid);
        },
        handlePasswordToggled: function (passwordOpened) {
            this.showPassword = passwordOpened;
        }
    }
}