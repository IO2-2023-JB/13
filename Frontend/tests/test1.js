const {Builder, By, Key, until } = require ("selenium-webdriver")

//to run: node tests/test1.js

async function example(){
    //launch the browser
    let driver = await new Builder().forBrowser("firefox").build();
    await driver.get("https://iofronttest.azurewebsites.net/");

    //navigate to registration page
    let button = await driver.findElement(By.xpath("/html/body/div/div/nav[1]/ul/li[4]/a"));
    await button.click();
    //wrong register
    let input = await driver.findElement(By.xpath('//*[@id="name"]'));
    await input.sendKeys('Krzysztof');
    input = await driver.findElement(By.xpath('//*[@id="surname"]'));
    await input.sendKeys('Kowalski');
    input = await driver.findElement(By.xpath('//*[@id="email"]'));
    await input.sendKeys('abdc123');
    input = await driver.findElement(By.xpath('//*[@id="password"]'));
    await input.sendKeys('Kicia345!');
    input = await driver.findElement(By.xpath('//*[@id="confirm_pwd"]'));
    await input.sendKeys('Kicia345!');
    // let checkbox = await driver.findElement(By.xpath('//*[@id="terms"]'));
    // await driver.executeScript("arguments[0].click();", checkbox);
    await input.sendKeys('', Key.ENTER);

    try{
        let element = await driver.findElement(By.xpath('/html/body/div/div/section/p/a'));
        console.log('ERROR Dokonano niepoprawnej rejestracji.');
        await driver.close();
        await driver.quit();
        return;
    }
    catch(err1){
        //proper register
        input = await driver.findElement(By.xpath('//*[@id="username"]'));
        await input.clear();
        await input.sendKeys('krzyskowal');
        input = await driver.findElement(By.xpath('//*[@id="email"]'));
        await input.clear();
        await input.sendKeys('krzyskowal@pw.edu.pl');

        input = await driver.findElement(By.xpath('//*[@id="confirm_pwd"]'));
        await input.sendKeys('', Key.ENTER);
        
        try{
            await driver.wait(until.elementLocated(By.xpath('/html/body/div/div/section/p/a')), 10000);
        }
        catch(err2){
            console.log('ERROR Nieudana rejestracja');
            await driver.close();
            await driver.quit();
            return;
        }
        try{
            let link = await driver.findElement(By.partialLinkText('Sign'));
            //navigate to login page
            await driver.executeScript("arguments[0].scrollIntoView()", link);
            await link.click();

            try{
                await driver.wait(until.elementLocated(By.xpath('//*[@id="email"]')), 10000);
            }
            catch(err3){
                console.log('ERROR Nieudana rejestracja');
                await driver.close();
                await driver.quit();
                return;
            }
            //log in
            input = await driver.findElement(By.xpath('//*[@id="email"]'));
            await input.sendKeys('krzyskowal@pw.edu.pl');
            input = await driver.findElement(By.xpath('//*[@id="password"]'));
            await input.sendKeys('Kicia345!', Key.ENTER);

            try{
                await driver.wait(until.elementLocated(By.xpath('/html/body/div/div/div/table/tr[1]/img')), 10000);
            }
            catch(err4){
                console.log('ERROR Nieudane logowanie');
                await driver.close();
                await driver.quit();
                return;
            }
            //logout
            button = await driver.findElement(By.xpath("/html/body/div/div/nav[1]/ul/li[4]/button"));
            await button.click();

            //check test result
            try{
                await driver.wait(until.elementLocated(By.xpath('/html/body/div/div/section/h1')), 10000);

                console.log('TEST SUCCEDED!');
            }
            catch(err5){
                console.log('ERROR Nieudane wylogowanie.');
                await driver.close();
                await driver.quit();
                return;
            }
        }
        catch(err0){
            console.log("ERROR Przycisk rejestracji nie dziala.");
            await driver.close();
            await driver.quit();
            return;
        }
    }
}
example()