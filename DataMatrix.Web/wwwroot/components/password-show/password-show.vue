<template>
    <img class="password-show-toggler"
         :src="eyeOpen ? '/icons/common/eye-slash.svg' : '/icons/common/eye.svg'"
         id="togglePassword"
         @click="togglePassword" />
</template>
<script src="./password-show.js"></script>
<style src="./password-show.scss" lang="scss"></style>