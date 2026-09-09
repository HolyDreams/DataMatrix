<template>
    <div :class="{ 'vw-invalid-area': !isValid, 'no-icon': icon === null }" class="vw vw-textbox">
        <label :class="{ 'on-top': focused || hasValue || inputType === 'date', 'vw-textbox-big-indent': showValidationIcon }" class="vw-textbox-label">{{ label }}</label>
        <input :placeholder="visiblePlaceholder" :ref="inputName" :type="inputType" :autocomplete="autocomplete" :readonly="readonly ? 'readonly' : null" :class="{ 'vw-textbox-big-indent': showValidationIcon, 'disabled': disabled }" :name="name" :value="internalValue" @keydown="handleKeydown" @input="handleInput" @focus="handleFocus" @blur="handleBlur" @keypress="handleKeypress" @paste="handlePaste" class="vw-textbox-input" :disabled="disabled === true" />
        <password-show v-if="showPasswordToggle" @toggle="handlePasswordToggled"></password-show>
        <validation-error :ref="inputName + '-validation'" v-if="name !== null" v-show="validationMode === 'label'" @change="handleValidChange" :for="name"></validation-error>
    </div>
</template>
<script src="./textbox.js"></script>
<style src="./textbox.scss" lang="scss"></style>