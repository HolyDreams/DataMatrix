export default {
    mounted: function() {
        var component = this;
        Array.from(component.$el.children).forEach(function(child) {
            child.addEventListener('click',
                function(event) {
                    var popup = component.$parent;

                    if (popup === undefined || popup === null)
                        return;

                    var closingReason = {
                        event: event,
                        closer: component,
                        isClosingCanceled: false
                    };

                    popup.$emit('closing', closingReason);
                    if (closingReason.isClosingCanceled)
                        return;

                    popup.showed = false;
                });
        });
    }
}