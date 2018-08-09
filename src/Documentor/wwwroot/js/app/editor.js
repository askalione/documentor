(function () {

    $(document).ready(function () {
        var editor = new SimpleMDE({
            element: document.getElementById("editor"),
            toolbar: [
                "bold", "italic", "heading-1", "heading-2", "heading-3", "strikethrough", "|",
                "code", "quote", "unordered-list", "ordered-list", "|",
                "link", "table", "horizontal-rule", "|",
                "preview", "side-by-side", "fullscreen", "|",
                "guide", "|",
                {
                    name: "save",
                    action: function customFunction(editor) {
                        $(document).find('.body-overlay').show();
                        $(document).find('form.page__editor-form').submit();
                    },
                    className: "la la-save",
                    title: "Save",
                }
            ]
        });
    });

}($));