SVG.bench.describe('Generate 10000 pathArrays', function(bench) {
  var data = 'M97.499,75.211l5.652,-4.874c-1.235,-3.156 -2.115,-6.44 -2.623,-9.791l-8.313,-1.582c-0.345,-3.501 -0.345,-7.027 0,-10.527l8.313,-1.582c0.508,-3.351 1.388,-6.635 2.623,-9.791l-6.408,-5.526c1.452,-3.204 3.215,-6.258 5.263,-9.117l7.99,2.787c2.116,-2.648 4.52,-5.052 7.168,-7.168l-2.787,-7.99c2.86,-2.049 5.913,-3.812 9.117,-5.263l5.526,6.408c3.156,-1.236 6.44,-2.115 9.791,-2.624l1.582,-8.312c3.501,-0.345 7.027,-0.345 10.527,0l1.582,8.312c3.351,0.509 6.635,1.388 9.791,2.624l5.526,-6.408c3.204,1.451 6.258,3.214 9.117,5.263l-2.787,7.99c2.648,2.116 5.052,4.52 7.168,7.168l7.99,-2.787c2.049,2.859 3.812,5.913 5.263,9.117l-6.408,5.526c1.236,3.156 2.115,6.44 2.624,9.791l8.312,1.582c0.345,3.5 0.345,7.026 0,10.527l-8.312,1.582c-0.509,3.351 -1.388,6.635 -2.624,9.791l6.408,5.526c-1.451,3.204 -3.214,6.257 -5.263,9.117l-7.99,-2.787c-2.116,2.648 -4.52,5.052 -7.168,7.168l2.787,7.99c-2.859,2.048 -5.913,3.811 -9.117,5.263l-5.526,-6.408c-3.156,1.235 -6.44,2.115 -9.791,2.624l-1.444,7.589l0.16,0.015l1.582,8.313c3.351,0.508 6.635,1.388 9.791,2.624l5.526,-6.409c3.204,1.452 6.258,3.215 9.117,5.264l-2.787,7.99c2.648,2.116 5.052,4.52 7.168,7.167l7.99,-2.786c2.049,2.859 3.812,5.913 5.263,9.117l-6.408,5.526c1.235,3.156 2.115,6.44 2.624,9.791l8.312,1.582c0.345,3.5 0.345,7.026 0,10.527l-8.312,1.581c-0.509,3.351 -1.389,6.635 -2.624,9.792l6.408,5.526c-1.451,3.204 -3.214,6.257 -5.263,9.116l-7.99,-2.786c-2.116,2.648 -4.52,5.052 -7.168,7.168l2.787,7.989c-2.859,2.049 -5.913,3.812 -9.117,5.264l-5.526,-6.408c-3.156,1.235 -6.44,2.115 -9.791,2.623l-1.582,8.313c-3.5,0.345 -7.026,0.345 -10.527,0l-1.582,-8.313c-3.351,-0.508 -6.635,-1.388 -9.791,-2.623l-5.526,6.408c-3.204,-1.452 -6.258,-3.215 -9.117,-5.264l2.787,-7.989c-2.648,-2.116 -5.052,-4.52 -7.168,-7.168l-7.99,2.786c-2.048,-2.859 -3.811,-5.912 -5.263,-9.116l6.408,-5.526c-1.235,-3.157 -2.115,-6.441 -2.624,-9.792l-8.312,-1.581c-0.345,-3.501 -0.345,-7.027 0,-10.527l8.312,-1.582c0.509,-3.351 1.389,-6.635 2.624,-9.791l-6.408,-5.526c0.034,-0.076 0.068,-0.151 0.103,-0.226l-7.783,-2.714c-2.116,2.648 -4.52,5.052 -7.168,7.167l2.787,7.99c-2.86,2.049 -5.913,3.812 -9.117,5.264l-5.526,-6.408c-3.156,1.235 -6.44,2.115 -9.791,2.623l-1.582,8.313c-3.501,0.345 -7.027,0.345 -10.527,0l-1.582,-8.313c-3.351,-0.508 -6.635,-1.388 -9.791,-2.623l-5.526,6.408c-3.204,-1.452 -6.258,-3.215 -9.117,-5.264l2.787,-7.99c-2.648,-2.115 -5.052,-4.519 -7.168,-7.167l-7.99,2.786c-2.049,-2.859 -3.812,-5.913 -5.263,-9.116l6.408,-5.527c-1.236,-3.156 -2.115,-6.44 -2.624,-9.791l-8.312,-1.581c-0.345,-3.501 -0.345,-7.027 0,-10.528l8.312,-1.581c0.509,-3.351 1.388,-6.635 2.624,-9.791l-6.408,-5.527c1.451,-3.204 3.214,-6.257 5.263,-9.116l7.99,2.786c2.116,-2.648 4.52,-5.052 7.168,-7.167l-2.787,-7.99c2.859,-2.049 5.913,-3.812 9.117,-5.264l5.526,6.408c3.156,-1.235 6.44,-2.115 9.791,-2.623l1.582,-8.313c3.5,-0.345 7.026,-0.345 10.527,0l1.582,8.313c3.351,0.508 6.635,1.388 9.791,2.623l5.526,-6.408c3.204,1.452 6.257,3.215 9.117,5.264l-2.787,7.99c2.648,2.115 5.052,4.519 7.168,7.167l7.99,-2.786c0.049,0.069 0.099,0.139 0.148,0.209Zm48.456,73.925c5.927,0 10.74,4.813 10.74,10.74c0,5.928 -4.813,10.74 -10.74,10.74c-5.928,0 -10.741,-4.812 -10.741,-10.74c0,-5.927 4.813,-10.74 10.741,-10.74Zm-5.402,-41.978l-0.16,-0.016l-1.582,-8.312c-3.351,-0.509 -6.635,-1.389 -9.791,-2.624l-5.526,6.408c-3.204,-1.452 -6.257,-3.215 -9.117,-5.263l2.787,-7.99c-2.648,-2.116 -5.052,-4.52 -7.168,-7.168l-7.99,2.787c-0.049,-0.07 -0.099,-0.139 -0.148,-0.209l-5.652,4.874c1.235,3.156 2.115,6.44 2.624,9.791l8.312,1.581c0.345,3.501 0.345,7.027 0,10.528l-8.312,1.581c-0.509,3.351 -1.389,6.635 -2.624,9.791l6.408,5.527c-0.034,0.075 -0.068,0.15 -0.103,0.225l7.783,2.714c2.116,-2.647 4.52,-5.051 7.168,-7.167l-2.787,-7.99c2.859,-2.049 5.913,-3.812 9.117,-5.264l5.526,6.409c3.156,-1.236 6.44,-2.116 9.791,-2.624l1.444,-7.589Zm-86.853,-11.617c5.928,0 10.74,4.812 10.74,10.74c0,5.928 -4.812,10.74 -10.74,10.74c-5.927,0 -10.74,-4.812 -10.74,-10.74c0,-5.928 4.813,-10.74 10.74,-10.74Zm91.957,-52.581c5.927,0 10.74,4.813 10.74,10.74c0,5.928 -4.813,10.74 -10.74,10.74c-5.928,0 -10.74,-4.812 -10.74,-10.74c0,-5.927 4.812,-10.74 10.74,-10.74Z'
  
  var data2 = 'M0.48858732019046247.35239897640279355L0.5168962223329727.32957811442929225C0.5107105368860513.31480120831760355.5063029229643583.2994249853547438.5037585276550173.2837350574775992L0.46212160205156927.2763278757701558C0.46039361704817827.25993562345804494.46039361704817827.2434263170734397.46212160205156927.22703874692422862L0.5037585276550173.2196315652167852C0.5063029229643582.20394163733964055.5107105368860513.1885654143767808.5168962223329727.17378850826509218L0.4848007791395535.14791487608093778C0.492073342110347.13291322615006.5009035959102843.11861390065414837.5111613155825881.1052275969236928L0.5511804465306873.11827678492536459C0.5617787545514856.10587841756676146.5738195544012016.09462249795570335.5870824653837506.0847150412597803L0.5731233517476615.047304559690581297C0.5874480969931638.037710807908943156.6027395121101284.02945615471664055.6187872337068381.022662336349067613L0.6464650456741969.052665636210823215C0.6622723519660869.046878482866701814.678720765737496.04276286167779995.6955047592052157.040379640761814675L0.7034284469598956.0014615027388882582C0.7209637382551767 -0.0001538434615339766.7386242458550512 -0.0001538434615339766.7561545284981485.0014615027388882582L0.7640782162528285.040379640761814675C0.7808622097205482.04276286167779995.7973106234919571.046878482866701814.8131179297838472.052665636210823215L0.8407957417512061.022662336349067617C0.8568434633479157.02945615471664055.872139887117064.037710807908943156.8864596237103826.04730455969058131L0.8725005100742934.0847150412597803C0.8857634210568424.09462249795570335.8978042209065583.10587841756676146.9084025289273567.11827678492536459L0.948421659875456.1052275969236928C0.9586843881999436.11861390065414837.9675146419998809.13291322615006.9747821963184906.14791487608093778L0.9426867531250714.17378850826509218C0.9488774472241766.1885654143767808.953280052493686.20394163733964055.9558294564552107.2196315652167852L0.9974613734064749.22703874692422862C0.9991893584098659.2434263170734397.9991893584098659.25993562345804494.9974613734064749.2763278757701558L0.9558294564552107.2837350574775992C0.953280052493686.2994249853547438.9488774472241766.31480120831760366.9426867531250714.32957811442929225L0.9747821963184906.3554517466134466C0.9675146419998809.37045339654432435.9586843881999436.38474803987733625.948421659875456.3981390257706916L0.9084025289273567.3850898377690198C0.8978042209065583.3974882051276229.8857634210568424.40874412473868105.8725005100742934.41865158143460407L0.8864596237103826.4560620630038031C0.8721398871170639.46565113262254143.8568434633479156.4739057858148441.8407957417512061.4807042863453168L0.8131179297838472.45070098648356116C0.7973106234919571.4564834576647828.7808622097205482.4606037610165844.7640782162528285.4629869819325697L0.756845722499505.4985199161789591L0.7576471068489037.4985901486224557L0.7655707946035836.5375129688082819C0.7823547880713033.5398915075613674.7988032018427124.5440118109131691.8146105081346023.5497989642572905L0.8422883201019612.519790982232635C0.8583360416986708.5265894827631077.8736324654678193.5348441359554104.8879522020611378.5444378877370485L0.8739930884250485.5818483693062474C0.8872559994075976.5917558260021705.8992967992573135.6030117456132287.9098951072781118.615405430808932L0.9499142382262111.6023609249701599C0.9601769665506987.6157472287006156.969007220350636.6300465541965272.9762747746692458.6450482041274048L0.9441793314758266.6709218363115593C0.9503650169227481.685698742423248.9547726308444411.7010749653861077.9573220348059658.7167648932632523L0.9989539517572301.7241720749706957C1.000681936760621.7405596451199068 1.000681936760621.757068951504512.9989539517572301.7734612038166229L0.9573220348059658.7808637033611664C0.9547726308444411.796553631238311.950365016922748.8119298542011708.9441793314758266.8267114424757592L0.9762747746692458.8525850746599137C0.969007220350636.8675867245907916.9601769665506987.8818813679238033.9499142382262111.8952676716542589L0.9098951072781118.8822231658154869C0.8992967992573135.89462153317409.8872559994075976.9058774527851481.8739930884250485.9157849094810713L0.8879522020611378.9531907088873705C0.8736324654678191.9627844606690087.8583360416986707.9710391138613113.8422883201019612.977837614391784L0.8146105081346023.9478343145300284C0.7988032018427124.9536167857112502.7823547880713033.9577370890630518.7655707946035836.9601156278161371L0.7576471068489037.9990384480019633C0.7401168242058064 1.0006537942023856.7224563166059317 1.0006537942023856.7049210253106508.9990384480019633L0.6969973375559708.9601156278161371C0.6802133440882511.9577370890630517.6637649303168421.95361678571125.647957624024952.9478343145300284L0.6202798120575933.977837614391784C0.6042320904608837.9710391138613113.5889356666917354.9627844606690087.5746159300984167.9531907088873705L0.5885750437345059.9157849094810713C0.5753121327519569.9058774527851481.563271332902241.89462153317409.5526730248814425.8822231658154869L0.5126538939333434.8952676716542589C0.5023961742610396.8818813679238033.49356592046110226.8675867245907916.4862933574903087.8525850746599138L0.518388800683728.8267114424757593C0.5122031152368065.8119298542011709.5077955013151135.7965536312383112.5052460973535888.7808637033611665L0.46361418040232455.773461203816623C0.46188619539893355.757068951504512.46188619539893355.7405596451199069.46361418040232455.7241720749706959L0.5052460973535888.7167648932632524C0.5077955013151135.7010749653861078.5122031152368065.6856987424232481.518388800683728.6709218363115594L0.4862933574903087.645048204127405C0.48646365166455596.6446923597470222.48663394583880315.644341197529539.4868092486652341.6439900353120559L0.4478269087191694.6312826452020677C0.43722860069837116.6436810125606708.4251878008486552.6549369321717289.4119248898661061.6648397067047522L0.42588400350219535.7022501882739512C0.411559258256693.7118439400555895.3962678431397284.720098593247892.3802201215430187.7268970937783648L0.35254230957565996.6968937939166092C0.3367350032837699.702676265097831.32028658951236094.7067965684496326.3035025960446412.7091751072027179L0.2955789082899612.7480979273885441C0.27804361699468016.7497132735889663.2603831093948056.7497132735889663.24285282675170825.7480979273885441L0.23492913899702825.7091751072027179C0.21814514552930853.7067965684496325.2016967317578995.7026762650978308.1858894254660095.6968937939166092L0.15821161349865073.7268970937783648C0.14216389190194106.720098593247892.12686746813279273.7118439400555895.1125477315394741.7022501882739512L0.1265068451755633.6648397067047522C0.11324393419301426.6549369321717289.10120313434329828.6436810125606708.09060482632250003.6312826452020677L0.05058569537440074.6443271510408397C0.04032296704991321.6309408473103841.03149271324997591.6166415218144725.024225158931366137.6016445540464946L0.056320602124785436.5757662396994404C0.050129908025680216.5609893335877518.04572730275617092.545613110624892.0431778987946462.5299231827477474L0.001545981843381972.5225206832032038C-0.00018200316000904808.5061284308910928 -0.00018200316000904808.48961912450648765.001545981843381972.4732268721943768L0.0431778987946462.4658243726498331C0.04572730275617092.4501344447726885.050129908025680216.4347582218098287.056320602124785436.4199813156981401L0.02422515893136614.39410300135108595C0.03149271324997591.3791013514202082.04032296704991321.3648067080871963.05058569537440075.3514204043567407L0.09060482632250003.36446491019551275C0.10120313434329828.35206654283690964.11324393419301426.34081062322585154.1265068451755633.33090784869282824L0.1125477315394741.2934973671236292C0.12686746813279273.2839036153419911.14216389190194106.2756489621496885.15821161349865073.2688504616192157L0.1858894254660095.29885376148097137C0.2016967317578995.2930712902997497.21814514552930853.2889509869479481.23492913899702825.2865724481948626L0.24285282675170825.2476496280090364C0.2603831093948056.24603428180861417.27804361699468016.24603428180861417.2955789082899612.2476496280090364L0.3035025960446412.2865724481948626C0.32028658951236094.28895098694794813.33673500328377.2930712902997497.35254230957565996.29885376148097137L0.3802201215430187.2688504616192157C0.39626784313972835.2756489621496884.411559258256693.2839036153419911.42588400350219535.2934973671236292L0.4119248898661061.33090784869282824C0.42518780084865515.3408106232258515.43722860069837116.35206654283690964.4478269087191694.36446491019551275L0.4878460396672687.3514204043567407C0.4880914636242721.3517434735968252.4883418962334592.35207122499980936.48858732019046247.3523989764027936ZM0.731286570405869.6985278687686304C0.7609728518989083.6985278687686304.7850794948592591.7210631188052454.7850794948592591.7488142983122096C0.7850794948592591.7765701599820733.7609728518989085.7991007278557888.731286570405869.7991007278557888C0.701595280260646.7991007278557888.6774886373002953.7765701599820733.6774886373002953.7488142983122096C0.6774886373002953.7210631188052455.701595280260646.6985278687686304.731286570405869.6985278687686304ZM0.7042298313092943.5019800345618924L0.7034284469598956.501905119955496L0.6955047592052157.46298698193256965C0.678720765737496.46060376101658435.6622723519660869.45648345766478277.6464650456741969.4507009864835611L0.6187872337068382.4807042863453167C0.6027395121101286.473905785814844.5874480969931639.4656511326225414.5731233517476615.45606206300380303L0.5870824653837508.418651581434604C0.5738195544012017.408744124738681.5617787545514857.3974882051276229.5511804465306874.3850898377690197L0.5111613155825881.39813902577069155C0.5109158916255848.39781127436770736.5106654590163977.3974882051276229.5104200350593944.39716045372463865L0.48211113291688407.41998131569813996C0.48829681836380556.4347582218098286.4927044322854986.4501344447726883.4952538362470233.465824372649833L0.5368857531982875.47322687219437665C0.5386137382016786.48961912450648754.5386137382016786.5061284308910927.5368857531982875.5225206832032036L0.4952538362470233.5299231827477473C0.4927044322854986.5456131106248919.48829681836380556.5609893335877517.48211113291688407.5757662396994403L0.5142065761103034.6016445540464944C0.5140362819360561.6019957162639775.5138659877618089.6023468784814607.513690684935378.6026980406989437L0.5526730248814427.615405430808932C0.563271332902241.6030117456132287.5753121327519569.5917558260021705.588575043734506.5818483693062474L0.5746159300984167.5444378877370485C0.5889356666917354.5348441359554102.6042320904608837.5265894827631077.6202798120575934.519790982232635L0.6479576240249522.5497989642572905C0.6637649303168422.544011810913169.6802133440882512.5398915075613674.6969973375559709.5375129688082819L0.7042298313092945.5019800345618926ZM0.2692133631947428.447587348155211C0.2989046533399659.447587348155211.32300628764813283.47011791602892633.32300628764813283.4978737776987901C0.32300628764813283.5256296393686539.2989046533399659.5481602072423692.2692133631947428.5481602072423692C0.23952708170170345.5481602072423692.21542043874135278.5256296393686539.21542043874135278.4978737776987901C0.21542043874135278.47011791602892633.23952708170170345.447587348155211.2692133631947428.447587348155211ZM0.7297939920551139.20139454072216306C0.7594802735481532.20139454072216306.783586916508504.2239297907587782.783586916508504.2516809702657422C0.783586916508504.279436831935606.7594802735481533.30196739980932136.7297939920551139.30196739980932136C0.7001027019098908.30196739980932136.6760010676017238.27943683193560603.6760010676017238.2516809702657422C0.6760010676017238.2239297907587782.7001027019098908.20139454072216306.7297939920551139.20139454072216306Z '

  var data3 = 'M10 10-45-30.5.5 .89L2e-2.5.5.5-.5C.5.5.5.5.5.5L-3-4z'
  
  bench.test('using SVG.js v2.5.3', function() {
    for (var i = 0; i < 10000; i++)
      new SVG.PathArray(data)
  })
  
  bench.test('using SVG.js v2.5.3 more data', function() {
    for (var i = 0; i < 10000; i++)
      new SVG.PathArray(data2)
  })
  
  bench.test('using SVG.js v2.5.3 complicated data', function() {
    for (var i = 0; i < 10000; i++)
      new SVG.PathArray(data3)
  })
})