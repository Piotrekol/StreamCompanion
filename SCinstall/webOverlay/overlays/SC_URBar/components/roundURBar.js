//=======USER CONFIGURATION
const SCRoundURBarSettings = {
  //hit colors, css color value
  hitColors: {
    hit300: '#add8e6c9',
    hit100: '#008000c9',
    hit50: '#ffa500c9',
  },
  //arc colors, css color value
  arcColors: {
    hit300: '#add8e6',
    hit100: '#008000',
    hit50: '#ffa500',
  },
  //color of current avg hit marker
  hitMarkerColor: '#00ffffc9',
  canvas: {
    width: 600,
    height: 100,
  },
  //arc settings - when changing you might need to also adjust canvas settings above
  arc: {
    //x-center of the arc
    x: 300,
    //y-center of the arc
    y: 720,
    //arc radius
    radius: 680,
    //how long should the arc be, 0.01 - 2
    length: 0.25,
    //width of the arc line
    width: 8,
  },
  //How many hits should be displayed at the same time
  amountOfHitErrors: 40,
  // add #type (eg. /#circle ) at the end of overlay url to change or change it here. accepted values: 'rectangle' | 'circle' | 'ellipse'
  hitElementType: getHitTypeFromUrlHash('ellipse'),
  //should UR bar hide itself when not playing? true/false
  hideWhenNotPlaying: false,
  //per-hitType hit configuration
  hitElements: {
    rectangle: {
      //width of hit element
      width: 3,
      //height of hit element
      height: 30,
      //how far away should the hit be drawn from arc, negative values inside, positive values outside
      distanceFromArc: 0,
      marker: {
        //how far away should current avg hit marker arrow be drawn from arc, negative values inside, positive values outside
        distanceFromArc: 5,
        //size of avg hit marker arrow
        size: 8,
      },
      //don't touch (:
      drawFn: () => canvasHelpers.arc.drawRectangleAroundArc,
    },
    circle: {
      width: 6,
      distanceFromArc: 12,
      marker: {
        distanceFromArc: 22,
        size: 8,
      },
      drawFn: () => canvasHelpers.arc.drawCircleAroundArc,
    },
    ellipse: {
      width: 2.5,
      height: 8,
      distanceFromArc: 13,
      marker: {
        distanceFromArc: 22,
        size: 8,
      },
      drawFn: () => canvasHelpers.arc.drawEllipseAroundArc,
    },
  },
};
//=======END OF USER CONFIGURATION



const canvasHelpers = (() => {
  function drawAroundArc(x, y, deg, yOffset, ctx, drawFn) {
    ctx.save();
    ctx.translate(x, y);
    ctx.rotate(degrees_to_radians(deg + 90));
    ctx.translate(0, yOffset);
    drawFn(ctx);
    ctx.restore();
  }
  function drawRectangleAroundArc(x, y, w, h, deg, yOffset, fillStyle, ctx) {
    drawAroundArc(x, y, deg, yOffset, ctx, () => drawRectangle(-1 * (w / 2), -1 * (h / 2), w, h, fillStyle, ctx));
  }
  function drawCircleAroundArc(x, y, w, h, deg, yOffset, fillStyle, ctx) {
    drawAroundArc(x, y, deg, yOffset, ctx, () => drawCircle(0, 0, w / 2, w, 0, 2 * Math.PI, fillStyle, ctx));
  }
  function drawEllipseAroundArc(x, y, w, h, deg, yOffset, fillStyle, ctx) {
    drawAroundArc(x, y, deg, yOffset, ctx, () => drawEllipse(0, 0, w, h, 0,0, 2 * Math.PI,0, fillStyle, ctx));
  }  
  function drawRectangle(x, y, w, h, fillStyle, ctx) {
    ctx.fillStyle = fillStyle;
    ctx.fillRect(x, y, w, h);
  }
  function drawCircle(x, y, radius, width, startAngle, endAngle, fillStyle, ctx) {
    ctx.beginPath();
    ctx.strokeStyle = fillStyle;
    ctx.lineWidth = width;
    ctx.arc(x, y, radius, startAngle, endAngle, false);
    ctx.stroke();
  }
  function drawEllipse(x,y,radiusX,radiusY,rotation,startAngle,endAngle,lineWidth,fillStyle,ctx){
    startAngle = startAngle * Math.PI;
    endAngle = endAngle * Math.PI;
    ctx.beginPath();
    ctx.ellipse(x, y, radiusX, radiusY, rotation, startAngle, endAngle, false);
    ctx.lineWidth = lineWidth;
    ctx.fillStyle = fillStyle;
    ctx.fill();
  }
  function drawArc(x, y, startAngle, endAngle, radius, strokeStyle, lineWidth, ctx) {
    startAngle = startAngle * Math.PI;
    endAngle = endAngle * Math.PI;
    ctx.beginPath();
    ctx.arc(x, y, radius, startAngle, endAngle, false);
    ctx.lineWidth = lineWidth;
    ctx.strokeStyle = strokeStyle;
    ctx.stroke();
  }
  function drawArrowAroundArc(x, y, deg, yOffset, fillStyle, p1, p2, size, ctx) {
    drawAroundArc(x, y, deg, yOffset, ctx, (ctx) => {
      var angle = Math.atan2(p2.y - p1.y, p2.x - p1.x);
      var hyp = Math.sqrt((p2.x - p1.x) * (p2.x - p1.x) + (p2.y - p1.y) * (p2.y - p1.y));

      ctx.save();
      ctx.translate(p1.x, p1.y);
      ctx.rotate(angle);

      // // line
      // ctx.beginPath();
      // ctx.moveTo(0, 0);
      // ctx.lineTo(hyp - size, 0);
      // ctx.stroke();

      // triangle
      ctx.fillStyle = fillStyle;
      ctx.beginPath();
      ctx.lineTo(hyp - size, size);
      ctx.lineTo(hyp, 0);
      ctx.lineTo(hyp - size, -size);
      ctx.fill();

      ctx.restore();
    });
  }
  function clearCanvas(canvas, context) {
    context.save();
    context.setTransform(1, 0, 0, 1, 0, 0);
    context.clearRect(0, 0, canvas.width, canvas.height);
    context.restore();
  }
  function degrees_to_radians(degrees) {
    return (degrees * Math.PI) / 180;
  }
  function radians_to_degrees(radians) {
    return (radians * 180) / Math.PI;
  }

  return {
    arc: {
      drawArc,
      drawAroundArc,
      drawRectangleAroundArc,
      drawCircleAroundArc,
      drawArrowAroundArc,
      drawEllipseAroundArc
    },
    degrees_to_radians,
    radians_to_degrees,
    drawRectangle,
    drawCircle,
    drawEllipse,
    clearCanvas,
  };
})();

function odToMs(od) {
  return {
    hit300: (159 - 12 * od) / 2,
    hit100: (279 - 16 * od) / 2,
    hit50: (399 - 20 * od) / 2,
  };
}

function scaleValue(value, from, to) {
  var scale = (to[1] - to[0]) / (from[1] - from[0]);
  var capped = Math.min(from[1], Math.max(from[0], value)) - from[0];
  return capped * scale + to[0];
}

function movingAverage(windowSize, values) {
  if (!(windowSize > 0)) {
    throw new Error('windowSize must be positive');
  }
  if (!(values != null && values.length)) {
    throw new Error('invalid array of values');
  }
  let sum = 0.0;
  const results = [];
  for (let i = 0; i < values.length; i++) {
    const val = values[i];
    sum += val;
    if (i >= windowSize) {
      sum -= values[i - windowSize];
    }
    results.push(sum / Math.min(i + 1, windowSize));
  }
  return results;
}

function getHitTypeFromUrlHash(fallbackValue){
  const hash = (window.location.hash || '').toLowerCase();
  return ['#rectangle', '#circle', '#ellipse'].indexOf(hash)>-1 ? hash.substr(1) : fallbackValue;
}

const roundurbar = {
  name: 'roundurbar',
  template: `
<div v-show="show" style="position:relative">
  <canvas ref="canvasBarRef" :width="width" :height="height" style="position:absolute"></canvas>
  <canvas ref="canvasHitsRef" :width="width" :height="height" style="position:absolute"></canvas>
  <canvas ref="canvasAvgHitsArrowRef" :width="width" :height="height" style="position:absolute"></canvas>
</div>
      `,
  setup(props, context) {
    const data = Vue.reactive({
      tokens: { hitErrors: '', unstableRate: '', mOD: '' },
      rws: {},
    });
    data.rws = watchTokens(['hitErrors', 'unstableRate', 'mOD'], (values) => Object.assign(data.tokens, values));
    const isPlayingOrWatching = Vue.computed(() => _IsInStatus(data.rws, data.tokens, [window.overlay.osuStatus.Playing, window.overlay.osuStatus.Watching]));
    const settings = SCRoundURBarSettings;

    //some calc
    const arcStart = 1.5 - settings.arc.length / 2;
    const arcEnd = 1.5 + settings.arc.length / 2;
    const arcDiff = arcEnd - arcStart;
    const rectDegOffset = -canvasHelpers.radians_to_degrees(arcDiff) * 1.572 + 90;
    const x = settings.arc.x;
    const y = settings.arc.y;
    const hitSettings = settings.hitElements[settings.hitElementType];
    let hitsContext, barContext, AvgHitsArrowContext, arrowInterval, hitMsWindows=odToMs(1);

    let arrowAvg = 0;
    const drawHitsArrow = () => {
      canvasHelpers.clearCanvas(canvasAvgHitsArrowRef.value, AvgHitsArrowContext);
      if (!data.tokens.hitErrors || data.tokens.hitErrors.length === 0) {
        return;
      }

      let avgErrorHits = data.tokens.hitErrors.slice(Math.max(data.tokens.hitErrors.length - 10, 0));
      avgErrorHits = movingAverage(2, avgErrorHits);
      for (var i = 0; i < avgErrorHits.length; i++) {
        arrowAvg = arrowAvg * 0.95 + avgErrorHits[i] * 0.05;
      }
      canvasHelpers.arc.drawArrowAroundArc(
        x,
        y,
        getDegForHit(arrowAvg),
        settings.arc.radius + hitSettings.marker.distanceFromArc,
        settings.hitMarkerColor,
        { x: 0, y: 5 },
        { x: 0, y: 0 },
        hitSettings.marker.size,
        AvgHitsArrowContext
      );
    };

    //canvas dom element refs
    const canvasBarRef = Vue.ref(null);
    const canvasHitsRef = Vue.ref(null);
    const canvasAvgHitsArrowRef = Vue.ref(null);

    Vue.onMounted(() => {
      barContext = canvasBarRef.value.getContext('2d');
      hitsContext = canvasHitsRef.value.getContext('2d');
      AvgHitsArrowContext = canvasAvgHitsArrowRef.value.getContext('2d');
      arrowInterval = setInterval(drawHitsArrow, 33);
    });

    const getColorForHit = (hit) => {
      let abs = Math.abs(hit);
      if (abs <= hitMsWindows.hit300) return settings.hitColors.hit300;
      if (abs <= hitMsWindows.hit100) return settings.hitColors.hit100;
      return settings.hitColors.hit50;
    }
    const getDegForHit = (hit) => {
      return (
        rectDegOffset +
        canvasHelpers.radians_to_degrees(
          scaleValue(hit.clamp(-hitMsWindows.hit50, hitMsWindows.hit50), [-hitMsWindows.hit50, hitMsWindows.hit50], [0, arcDiff * 3.142])
        )
      );
    };
    
    //draw hit elements around arc circumference
    const drawShapeFn = hitSettings.drawFn()
    const drawHits = (hits, height = null) => {
      hits = hits || [];
      height = height || hitSettings.height;
      hits.forEach((hitError) => {
        drawShapeFn(
          x,
          y,
          hitSettings.width,
          height,
          getDegForHit(hitError),
          settings.arc.radius + hitSettings.distanceFromArc,
          getColorForHit(hitError),
          hitsContext
        );
      });
    };

    //Update arc whenever map OD value changes
    Vue.watch(
      () => data.tokens.mOD,
      () => {
        hitMsWindows = odToMs(data.tokens.mOD);
        let arcTotal = Math.abs(arcStart - arcEnd);

        //radians taken on arc by each hit
        let h300p = (hitMsWindows.hit300 / hitMsWindows.hit50) * arcTotal;
        let h100p = ((hitMsWindows.hit100 - hitMsWindows.hit300) / hitMsWindows.hit50) * arcTotal;
        let h50p = arcTotal - h300p - h100p;

        //draw arc sections
        canvasHelpers.clearCanvas(canvasBarRef.value, barContext);
        const drawArc = canvasHelpers.arc.drawArc;
        const radius = settings.arc.radius;
        const arcColors = settings.arcColors;
        const arcWidth = settings.arc.width;

        let newArcStart = arcStart + h50p / 2;
        drawArc(x, y, arcStart, newArcStart, radius, arcColors.hit50, arcWidth, barContext);

        let newArcEnd = newArcStart + h100p / 2;
        drawArc(x, y, newArcStart, newArcEnd, radius, arcColors.hit100, arcWidth, barContext);
        newArcStart = newArcEnd;

        newArcEnd = newArcStart + h300p;
        drawArc(x, y, newArcStart, newArcEnd, radius, arcColors.hit300, arcWidth, barContext);
        newArcStart = newArcEnd;

        newArcEnd = newArcStart + h100p / 2;
        drawArc(x, y, newArcStart, newArcEnd, radius, arcColors.hit100, arcWidth, barContext);
        newArcStart = newArcEnd;

        newArcEnd = newArcStart + h50p / 2;
        drawArc(x, y, newArcStart, newArcEnd, radius, arcColors.hit50, arcWidth, barContext);
        newArcStart = newArcEnd;

        canvasHelpers.clearCanvas(canvasHitsRef.value, hitsContext);
        drawHits([0, -hitMsWindows.hit50, hitMsWindows.hit50, -hitMsWindows.hit100, hitMsWindows.hit100, -hitMsWindows.hit300, hitMsWindows.hit300]);
      }
    );

    //Update drawn hitErrors whenever these change
    Vue.watch(
      () => data.tokens.hitErrors,
      () => {
        canvasHelpers.clearCanvas(canvasHitsRef.value, hitsContext);
        if (!data.tokens.hitErrors || !(x && y)) return;
        drawHits(data.tokens.hitErrors.slice(Math.max(data.tokens.hitErrors.length - settings.amountOfHitErrors, 0)));
      }
    );

    return {
      canvasBarRef,
      canvasHitsRef,
      canvasAvgHitsArrowRef,
      width: settings.canvas.width,
      height: settings.canvas.height,
      show: Vue.computed(() => (settings.hideWhenNotPlaying ? isPlayingOrWatching.value : true)),
    };
  },
};

export default roundurbar;
