<template>
    <div :class="{ 'showed': showed }" class="vw vw-popup" :style="popupStyle">
        <slot></slot>
    </div>
</template>
<script src="./popup.js"></script>
<style src="./popup.scss" lang="scss"></style>