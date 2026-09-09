export default {
    data: function () {
        return {
            internalValue: false
        };
    },
    computed: {
        eyeOpen: function () {
            return this.internalValue;
        }
    },
    methods: {
        togglePassword: function () {
            this.internalValue = !this.internalValue;
            const isOpened = this.internalValue;
            this.$emit('toggle', isOpened);
        }
    }
}