/**
 * @license Copyright (c) 2003-2019, CKSource - Frederico Knabben. All rights reserved.
 * For licensing, see https://ckeditor.com/legal/ckeditor-oss-license
 */
setTimeout(function () {
    CKEDITOR.editorConfig = function (config) {
        config.language = 'vi';
        config.height = 200;
        config.toolbarCanCollapse = true;
        config.allowedContent = true;
        config.extraAllowedContent = '*(*);*{*}';
        config.extraAllowedContent = 'span;ul;li;table;td;style;*[id];*(*);*{*}';
        //config.removePlugins = 'cloudservices,flash,format,iframe,smiley,wsc,tab,stylescombo,specialchar,showborders,showblocks,scayt,pagebreak,newpage,list,justify,about,a11yhelp,bidi,language,elementspath,forms,blockquote';

        config.toolbarGroups = [
            { name: 'document', groups: ['mode', 'document', 'doctools'] },
            { name: 'clipboard', groups: ['clipboard', 'undo'] },
            { name: 'editing', groups: ['find', 'selection', 'spellchecker', 'editing'] },
            { name: 'forms', groups: ['forms'] },
            { name: 'basicstyles', groups: ['basicstyles', 'cleanup'] },
            { name: 'paragraph', groups: ['list', 'indent', 'blocks', 'align', 'bidi', 'paragraph'] },
            { name: 'links', groups: ['links'] },
            { name: 'insert', groups: ['insert'] },
            { name: 'styles', groups: ['styles'] },
            { name: 'colors', groups: ['colors'] },
            { name: 'tools', groups: ['tools'] },
            { name: 'others', groups: ['others'] }
        ];
    };
}, 10);