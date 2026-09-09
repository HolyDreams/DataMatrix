import { allComponents } from '../mixins/global.js'

export default {
    data: function() {
        return {
            showed: false
        };
    },
    computed: {
        ref: function() {
            return this.$options._parentVnode.data.ref;
        },
        popupStyle: function() {
            return {
                display: this.showed ? null : 'none'
            };
        }
    },
    created: function() {
        var popupRef = this.ref;
        var popupOpener =
            allComponents.find(component => component.$options._componentTag === 'popup-opener' &&
                component.forPopup === popupRef);
        document.addEventListener(popupOpener.openWhenMouseEnter ? 'mousemove' : 'click',
            this.handleDocumentMouseEvent);
    },
    methods: {
        handleDocumentMouseEvent: function(event) {
            if (!this.showed)
                return;

            var parent = event.target.parentElement;
            while (parent !== null && parent !== undefined) {
                var forPopup = parent.getAttribute('data-for-popup');
                if (forPopup === this.ref)
                    return;

                parent = parent.parentElement;
            }

            var popup = this.$el;
            if (popup.contains(event.target))
                return;

            var closingReason = {
                event: event,
                isClosingCanceled: false
            };

            this.$emit('closing', closingReason);
            if (closingReason.isClosingCanceled)
                return;

            this.showed = false;
            this.$emit('closed');
        }
    }
}