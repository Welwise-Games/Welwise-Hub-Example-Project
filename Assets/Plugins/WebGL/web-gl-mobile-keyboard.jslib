const mobileKeyboard = {
    $mobileKeyboardInstance: {
        shouldReplaceEmojiTextToImage: true,
        shouldCloseKeyboardAfterSubmit: false,
        mobileKeyboardEventsObserverName: 'MobileKeyboardEventsObserver',
        getInputDiv: function(shouldSetInputDiv = false, startText = null, color = null, backgroundColor = null, top = null, bottom = null, left = null, width = null, height = null, transform = null, position = null, border = null, fontSize = null) {
            var inputDiv = document.getElementById('unityMobileKeyboardDiv');
            if (!inputDiv) {
                inputDiv = document.createElement('div');
                inputDiv.id = 'unityMobileKeyboardDiv';
                inputDiv.contentEditable = true;
                inputDiv.style.zIndex = '10000';
                inputDiv.style.overflowX = 'auto';
                inputDiv.style.whiteSpace = 'nowrap';
                inputDiv.style.outline = 'none';
                inputDiv.style.padding = '5px';
                inputDiv.style.userSelect = 'text';
                inputDiv.style.webkitUserModify = 'read-write-plaintext-only'; // для iOS
                inputDiv.style.border = '1px solid #ccc';
                inputDiv.style.minHeight = '60px';
                var self = this;
                inputDiv.addEventListener('keydown', (e) => {
                    if (e.key === 'Enter') {
                        e.preventDefault();
                        unityInstance.SendMessage(self.mobileKeyboardEventsObserverName, 'OnSubmit', '');
                        inputDiv.innerHTML = '';
                        if (self.shouldCloseKeyboardAfterSubmit) self.closeKeyboard();
                    }
                });
                inputDiv.addEventListener('input', () => {
                    if (self.shouldReplaceEmojiTextToImage) {
                        const marker = self.saveSelectionMarker(inputDiv);
                        self.replaceEmojiTextToImage(inputDiv);
                        self.restoreSelectionMarker(marker);
                    }
                    var text = self.shouldReplaceEmojiTextToImage ? self.getTextWithSprites(inputDiv) : inputDiv.textContent;
                    unityInstance.SendMessage(self.mobileKeyboardEventsObserverName, 'OnChangeValue', text);
                });
                document.body.appendChild(inputDiv);
            }
            if (shouldSetInputDiv) {
                inputDiv.style.position = position ? position : 'absolute';
                inputDiv.style.top = top ? top : '25%';
                inputDiv.style.left = left ? left : '50%';
                inputDiv.style.bottom = bottom ? bottom : 'auto';
                inputDiv.style.width = width ? width : '50%';
                inputDiv.style.height = height ? height : '60px';
                inputDiv.style.fontSize = fontSize ? fontSize : '16px';
                inputDiv.style.border = border ? border : '1px solid #ccc';
                inputDiv.style.backgroundColor = backgroundColor ? backgroundColor : 'white';
                inputDiv.style.color = color ? color : '#000';
                inputDiv.style.transform = transform ? transform : 'none';
                inputDiv.textContent = startText ? startText : '';
                if (this.shouldReplaceEmojiTextToImage) {
                    this.replaceEmojiTextToImage(inputDiv);
                }
                this.setCursorToEnd(inputDiv);
            }
            return inputDiv;
        },
        saveSelectionMarker: function(container) {
            const sel = window.getSelection();
            if (sel.rangeCount === 0) return null;
            const range = sel.getRangeAt(0);
            const marker = document.createElement('span');
            marker.id = 'selection-marker';
            marker.style.cssText = 'position: absolute; width: 0; height: 0; overflow: hidden;';
            marker.appendChild(document.createTextNode('\u200b')); // zero-width space
            range.insertNode(marker);
            return marker;
        },
        restoreSelectionMarker: function(marker) {
            if (!marker) return;
            const sel = window.getSelection();
            const range = document.createRange();
            range.setStartAfter(marker);
            range.collapse(true);
            sel.removeAllRanges();
            sel.addRange(range);
            marker.parentNode.removeChild(marker);
        },
        setCursorToEnd: function(element) {
            var sel = window.getSelection();
            var range = document.createRange();

            function getLastTextNode(node) {
                if (node.nodeType === 3) {
                    return node;
                }
                for (var i = node.childNodes.length - 1; i >= 0; i--) {
                    var textNode = getLastTextNode(node.childNodes[i]);
                    if (textNode) return textNode;
                }
                return null;
            }
            var lastTextNode = getLastTextNode(element);
            if (lastTextNode) {
                range.setStart(lastTextNode, lastTextNode.length);
            } else {
                var textNode = document.createTextNode('');
                element.appendChild(textNode);
                range.setStart(textNode, 0);
            }
            range.collapse(true);
            sel.removeAllRanges();
            sel.addRange(range);
        },
        openKeyboard: function(shouldSetInputDiv = false, shouldCloseKeyboardAfterSubmit = false, shouldReplaceEmojiTextToImage = true, startText = null, color = null, backgroundColor = null, top = null, bottom = null, left = null, width = null, height = null, transform = null, position = null, border = null, fontSize = null) {
            this.shouldReplaceEmojiTextToImage = shouldReplaceEmojiTextToImage;
            this.shouldCloseKeyboardAfterSubmit = shouldCloseKeyboardAfterSubmit;
            var inputDiv = this.getInputDiv(true, startText, color, backgroundColor, top, bottom, left, width, height, transform, position, border, fontSize);
            inputDiv.style.display = 'block';
            inputDiv.focus();
            this.setCursorToEnd(inputDiv);
        },
        closeKeyboard: function() {
            this.getInputDiv().style.display = 'none';
        },
        replaceEmojiTextToImage: function(element) {
            var regex = /<sprite=(\d+)>/g;

            function processNode(node) {
                if (node.nodeType === Node.TEXT_NODE) {
                    var text = node.nodeValue;
                    var parent = node.parentNode;
                    var match;
                    var lastIndex = 0;
                    var fragment = document.createDocumentFragment();
                    while ((match = regex.exec(text)) !== null) {
                        if (match.index > lastIndex) {
                            fragment.appendChild(document.createTextNode(text.substring(lastIndex, match.index)));
                        }
                        var img = document.createElement('img');
                        img.src = `emojies/emoji${match[1]}.png`;
                        img.alt = 'emoji';
                        img.style.width = '1em';
                        img.style.height = '1em';
                        img.style.verticalAlign = 'middle';
                        fragment.appendChild(img);
                        lastIndex = regex.lastIndex;
                    }
                    if (lastIndex < text.length) {
                        fragment.appendChild(document.createTextNode(text.substring(lastIndex)));
                    }
                    if (fragment.childNodes.length > 0) {
                        parent.replaceChild(fragment, node);
                    }
                } else if (node.nodeType === Node.ELEMENT_NODE) {
                    var child = node.firstChild;
                    while (child) {
                        var next = child.nextSibling;
                        processNode(child);
                        child = next;
                    }
                }
            }
            processNode(element);
        },
        getTextWithSprites: function(element) {
            var clone = element.cloneNode(true);
            var imgs = clone.querySelectorAll('img');
            imgs.forEach((img) => {
                var src = img.getAttribute('src');
                var regex = /emojies\/emoji(\d+)\.png/;
                var match = src.match(regex);
                if (match) {
                    var spriteIndex = match[1];
                    var spriteText = document.createTextNode(`<sprite=${spriteIndex}>`);
                    img.parentNode.replaceChild(spriteText, img);
                }
            });
            return clone.textContent;
        },
        insertTextAtCursor: function(text, shouldReplaceEmojiTextToImage) {
            var inputDiv = this.getInputDiv();

            if (inputDiv.style.display !== 'block')
            {
                inputDiv.style.display = 'block';
                inputDiv.focus();
                this.setCursorToEnd(inputDiv);
            }
            
            var sel = window.getSelection();
            if (!sel.rangeCount) return;
            var range = sel.getRangeAt(0);
            range.deleteContents();
            var tempSpan = document.createElement('span');
            tempSpan.className = 'temp-insert-container';
            tempSpan.textContent = UTF8ToString(text);
            range.insertNode(tempSpan);
            if (shouldReplaceEmojiTextToImage) this.replaceEmojiTextToImage(tempSpan);
            var newRange = document.createRange();
            newRange.setStartAfter(tempSpan);
            newRange.collapse(true);
            sel.removeAllRanges();
            sel.addRange(newRange);
            while (tempSpan.firstChild) {
                tempSpan.parentNode.insertBefore(tempSpan.firstChild, tempSpan);
            }
            tempSpan.parentNode.removeChild(tempSpan);
            this.closeKeyboard();
            var resultText = shouldReplaceEmojiTextToImage ? this.getTextWithSprites(inputDiv) : inputDiv.textContent;
            unityInstance.SendMessage(this.mobileKeyboardEventsObserverName, 'OnChangeValue', resultText);
        }
    },
    // External C# calls
    OpenKeyboard: function(shouldReplaceEmojiTextToImage, shouldCloseKeyboardAfterSubmit, startText, color, backgroundColor, top, bottom, left, width, height, transform, position, border, fontSize) {
        mobileKeyboardInstance.openKeyboard(true, shouldCloseKeyboardAfterSubmit, shouldReplaceEmojiTextToImage, GetString(startText), GetString(color), GetString(backgroundColor), GetString(top), GetString(bottom), GetString(left), GetString(width), GetString(height), 
        GetString(transform), GetString(position), GetString(border), GetString(fontSize));

        function GetString(pointerOrString) {
            if (pointerOrString === null) return null;
            if (typeof pointerOrString === 'string') {
                return pointerOrString;
            } else if (typeof pointerOrString === 'number') {
                return UTF8ToString(pointerOrString);
            } else {
                throw new Error('Invalid input for UTF8ToString');
            }
        }
    },
    InsertTextAtCursor: function(text, shouldReplaceEmojiTextToImage) {
        mobileKeyboardInstance.insertTextAtCursor(text, shouldReplaceEmojiTextToImage);
    },
    CloseKeyboard: function() {
        mobileKeyboardInstance.closeKeyboard();
    },
};
autoAddDeps(mobileKeyboard, '$mobileKeyboardInstance');
mergeInto(LibraryManager.library, mobileKeyboard);