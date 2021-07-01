import {AtlantisPage} from './app.po';

describe('Babylon App', () => {
    let page: AtlantisPage;

    beforeEach(() => {
        page = new AtlantisPage();
    });

    it('should display welcome message', () => {
        page.navigateTo();
        expect(page.getTitleText()).toEqual('Welcome to Atlantis!');
    });
});
