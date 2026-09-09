<template>
    <span class="vw-validation-error" :class="{ 'warning': isWarning }" v-html="validationError"></span>
</template>
<script src="./validation-error.js"></script>
<style src="./validation-error.scss" lang="scss"></style>