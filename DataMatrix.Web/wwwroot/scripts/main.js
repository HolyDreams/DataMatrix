var getModel = function () {
    let querySelector = document.querySelector('#model-data');
    if (querySelector === null)
        return {};

    let htmlData = querySelector.innerHTML;
    htmlData = htmlData.split('&lt;').join('<');
    htmlData = htmlData.split('&gt;').join('>');
    htmlData = htmlData.split('&amp;').join('&');

    return JSON.parse(htmlData);
}

export { getModel }