export default {
    data: function () {
        return {
            parentForm: null,
            validationError: null,
            isWarning: false,
            isValid: true
        }
    },
    props: {
        for: {
            type: String,
            requierd: true
        }
    },
    methods: {
        handleParentFormInvalid: function (errors) {
            var errorKey = this.for.toLocaleLowerCase();
            var error = errors.find(item => item.field.toLocaleLowerCase() === errorKey);
            this.validate(error);
        },
        validate(error) {
            if (error === undefined || error === null) {
                this.validationError = null;
                this.isValid = true;
                this.$emit('change', this);
                return;
            }

            this.isValid = false;
            this.validationError = error.error;
            this.isWarning = error.isWarning;
            this.$emit('change', this);
        },
        handleParentFormStartSubmit: function () {
            this.isValid = true;
        },
        handleParentFormSubmit: function () {
            this.isValid = true;
            this.validationError = null;
        }
    },
    mounted: function () {
        var parent = this.$parent;

        while (parent !== undefined && parent !== null) {
            if (parent.$options._componentTag === 'ajax-form') {
                this.parentForm = parent;
                this.parentForm.$on('start', this.handleParentFormStartSubmit);
                this.parentForm.$on('invalid', this.handleParentFormInvalid);
                this.parentForm.$on('success', this.handleParentFormSubmit);
                this.parentForm.$on('error', this.handleParentFormSubmit);
                break;
            }

            parent = parent.$parent;
        }
    }
}