// NorthwindApp Swagger UI Custom JavaScript

(function() {
    'use strict';

    // Wait for Swagger UI to load
    function waitForSwaggerUI() {
        if (typeof SwaggerUIBundle !== 'undefined') {
            initializeCustomFeatures();
        } else {
            setTimeout(waitForSwaggerUI, 100);
        }
    }

    function initializeCustomFeatures() {
        // Add custom header with app info
        addCustomHeader();
        
        // Add response time tracking
        addResponseTimeTracking();
        
        // Add copy button functionality
        addCopyButtons();
        
        // Add search functionality
        addSearchFunctionality();
        
        // Add dark mode toggle
        addDarkModeToggle();
        
        // Add keyboard shortcuts
        addKeyboardShortcuts();
    }

    function addCustomHeader() {
        const header = document.createElement('div');
        header.className = 'custom-header';
        header.innerHTML = `
            <div style="background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; padding: 15px; margin-bottom: 20px; border-radius: 8px;">
                <h3 style="margin: 0; color: white;">🚀 NorthwindApp API Documentation</h3>
                <p style="margin: 5px 0 0 0; opacity: 0.9;">Modern .NET 9.0 API with advanced features</p>
            </div>
        `;
        
        const infoSection = document.querySelector('.swagger-ui .info');
        if (infoSection) {
            infoSection.parentNode.insertBefore(header, infoSection);
        }
    }

    function addResponseTimeTracking() {
        // Track API response times
        const originalFetch = window.fetch;
        window.fetch = function(...args) {
            const startTime = performance.now();
            return originalFetch.apply(this, args).then(response => {
                const endTime = performance.now();
                const responseTime = endTime - startTime;
                
                // Log response time to console
                console.log(`API Response Time: ${responseTime.toFixed(2)}ms`);
                
                return response;
            });
        };
    }

    function addCopyButtons() {
        // Add copy buttons to code blocks
        const codeBlocks = document.querySelectorAll('pre code');
        codeBlocks.forEach(block => {
            const copyButton = document.createElement('button');
            copyButton.textContent = '📋 Copy';
            copyButton.className = 'copy-button';
            copyButton.style.cssText = `
                position: absolute;
                top: 5px;
                right: 5px;
                background: #4990e2;
                color: white;
                border: none;
                border-radius: 4px;
                padding: 5px 10px;
                font-size: 12px;
                cursor: pointer;
                opacity: 0;
                transition: opacity 0.3s;
            `;
            
            const container = block.parentElement;
            container.style.position = 'relative';
            container.appendChild(copyButton);
            
            container.addEventListener('mouseenter', () => {
                copyButton.style.opacity = '1';
            });
            
            container.addEventListener('mouseleave', () => {
                copyButton.style.opacity = '0';
            });
            
            copyButton.addEventListener('click', () => {
                navigator.clipboard.writeText(block.textContent).then(() => {
                    copyButton.textContent = '✅ Copied!';
                    setTimeout(() => {
                        copyButton.textContent = '📋 Copy';
                    }, 2000);
                });
            });
        });
    }

    function addSearchFunctionality() {
        // Add search box for endpoints
        const searchBox = document.createElement('div');
        searchBox.innerHTML = `
            <div style="margin: 20px 0; padding: 15px; background: #f8f9fa; border-radius: 8px;">
                <input type="text" id="endpoint-search" placeholder="🔍 Search endpoints..." 
                       style="width: 100%; padding: 10px; border: 1px solid #ddd; border-radius: 4px; font-size: 14px;">
            </div>
        `;
        
        const infoSection = document.querySelector('.swagger-ui .info');
        if (infoSection) {
            infoSection.parentNode.insertBefore(searchBox, infoSection.nextSibling);
        }
        
        // Add search functionality
        const searchInput = document.getElementById('endpoint-search');
        if (searchInput) {
            searchInput.addEventListener('input', (e) => {
                const searchTerm = e.target.value.toLowerCase();
                const endpoints = document.querySelectorAll('.opblock');
                
                endpoints.forEach(endpoint => {
                    const title = endpoint.querySelector('.opblock-summary-description')?.textContent.toLowerCase() || '';
                    const method = endpoint.querySelector('.opblock-summary-method')?.textContent.toLowerCase() || '';
                    
                    if (title.includes(searchTerm) || method.includes(searchTerm)) {
                        endpoint.style.display = 'block';
                    } else {
                        endpoint.style.display = 'none';
                    }
                });
            });
        }
    }

    function addDarkModeToggle() {
        // Add dark mode toggle button
        const toggleButton = document.createElement('button');
        toggleButton.innerHTML = '🌙';
        toggleButton.id = 'dark-mode-toggle';
        toggleButton.style.cssText = `
            position: fixed;
            top: 20px;
            right: 20px;
            z-index: 1000;
            background: #2c3e50;
            color: white;
            border: none;
            border-radius: 50%;
            width: 50px;
            height: 50px;
            font-size: 20px;
            cursor: pointer;
            box-shadow: 0 2px 10px rgba(0,0,0,0.2);
            transition: all 0.3s ease;
        `;
        
        document.body.appendChild(toggleButton);
        
        let isDarkMode = false;
        toggleButton.addEventListener('click', () => {
            isDarkMode = !isDarkMode;
            toggleButton.innerHTML = isDarkMode ? '☀️' : '🌙';
            
            if (isDarkMode) {
                document.body.classList.add('dark-mode');
            } else {
                document.body.classList.remove('dark-mode');
            }
        });
    }

    function addKeyboardShortcuts() {
        // Add keyboard shortcuts
        document.addEventListener('keydown', (e) => {
            // Ctrl/Cmd + K: Focus search
            if ((e.ctrlKey || e.metaKey) && e.key === 'k') {
                e.preventDefault();
                const searchInput = document.getElementById('endpoint-search');
                if (searchInput) {
                    searchInput.focus();
                }
            }
            
            // Ctrl/Cmd + D: Toggle dark mode
            if ((e.ctrlKey || e.metaKey) && e.key === 'd') {
                e.preventDefault();
                const toggleButton = document.getElementById('dark-mode-toggle');
                if (toggleButton) {
                    toggleButton.click();
                }
            }
            
            // Escape: Clear search
            if (e.key === 'Escape') {
                const searchInput = document.getElementById('endpoint-search');
                if (searchInput) {
                    searchInput.value = '';
                    searchInput.dispatchEvent(new Event('input'));
                }
            }
        });
    }

    // Add custom styles for dark mode
    const darkModeStyles = `
        <style>
            .dark-mode .swagger-ui {
                background-color: #1a1a1a !important;
                color: #ffffff !important;
            }
            
            .dark-mode .swagger-ui .opblock {
                background-color: #2d2d2d !important;
                border-color: #444 !important;
            }
            
            .dark-mode .swagger-ui .model {
                background-color: #2d2d2d !important;
                color: #ffffff !important;
            }
            
            .dark-mode .swagger-ui .parameters-container .parameter {
                background-color: #2d2d2d !important;
                color: #ffffff !important;
            }
            
            .dark-mode .swagger-ui .info {
                background: linear-gradient(135deg, #2c3e50 0%, #34495e 100%) !important;
            }
        </style>
    `;
    
    document.head.insertAdjacentHTML('beforeend', darkModeStyles);

    // Initialize when DOM is ready
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', waitForSwaggerUI);
    } else {
        waitForSwaggerUI();
    }

})();
